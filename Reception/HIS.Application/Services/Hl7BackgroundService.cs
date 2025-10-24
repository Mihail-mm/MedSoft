using System.Net;
using System.Net.Sockets;
using System.Text;
using HIS.Application.Contracts;
using HIS.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHapi.Base.Parser;

namespace HIS.Application.Services;

public class Hl7BackgroundService : BackgroundService
{
    private readonly int _port;
    private readonly ILogger<Hl7BackgroundService> _logger;
    private readonly IPatientService _patientService;

    public Hl7BackgroundService(IConfiguration configuration, ILogger<Hl7BackgroundService> logger,
        IPatientService patientService)
    {
        _port = Convert.ToInt32(configuration["HIS:Port"] ?? "2575");
        _logger = logger;
        _patientService = patientService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = await listener.AcceptTcpClientAsync(stoppingToken);
                _ = ProcessClientAsync(client, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        listener.Stop();
    }

    private async Task ProcessClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        using (client)
        await using (var stream = client.GetStream())
        {
            var buffer = new byte[4096];
            var bytesRead = await stream.ReadAsync(buffer, cancellationToken);
            var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            LogHl7MessageDetails(message);

            await ProcessHl7Message(message);

            var ackMessage = CreateAckMessage();
            var ackData = Encoding.UTF8.GetBytes(ackMessage);
            await stream.WriteAsync(ackData, cancellationToken);
        }
    }

    private async Task ProcessHl7Message(string hl7Message)
    {
        var parser = new PipeParser();
        var parsedMessage = parser.Parse(hl7Message);

        if (parsedMessage is NHapi.Model.V251.Message.ADT_A01 adt01Message)
        {
            await ProcessPatientAdmission(adt01Message);
        }
        else if (parsedMessage is NHapi.Model.V251.Message.ADT_A03 adt03Message)
        {
            await ProcessPatientDeletion(adt03Message);
        }
    }

    private async Task ProcessPatientAdmission(NHapi.Model.V251.Message.ADT_A01 message)
    {
        var pid = message.PID;
        var patient = new Patient(
            Id: Convert.ToInt32(pid.GetPatientIdentifierList(0).IDNumber.Value),
            Name: pid.GetPatientName(0).GivenName.Value,
            Surname: pid.GetPatientName(0).FamilyName.Surname.Value,
            BirthDate: DateOnly.FromDateTime(DateTime.ParseExact(
                pid.DateTimeOfBirth.Time.Value,
                "yyyyMMdd",
                System.Globalization.CultureInfo.InvariantCulture)),
            PatientStatus.NotArrived
        );

        await _patientService.AddPatient(patient);
    }

    private async Task ProcessPatientDeletion(NHapi.Model.V251.Message.ADT_A03 message)
    {
        var patientId = message.PID.GetPatientIdentifierList(0).IDNumber.Value;
        await _patientService.DeletePatient(Convert.ToInt32(patientId));
    }

    private string CreateAckMessage()
    {
        return "MSH|^~\\&|HIS_SRV|HOSPITAL|RECEPTION_API|HOSPITAL|" +
               DateTime.Now.ToString("yyyyMMddHHmmss") +
               "||ACK^A01|" + Guid.NewGuid() + "|P|2.5.1\r\n" +
               "MSA|AA|Message received successfully";
    }

    private void LogHl7MessageDetails(string hl7Message)
    {
        _logger.LogInformation("Получено сообщение:");
        var segments = hl7Message.Split('\r');
        var message = new StringBuilder();
        foreach (var segment in segments)
        {
            if (!string.IsNullOrEmpty(segment.Trim()))
            {
                message.AppendLine(segment.Trim());
            }
        }
        _logger.LogInformation(message.ToString());
    }
}