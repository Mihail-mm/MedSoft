using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reception.Application.Contracts;

namespace Reception.Application.Services;

public class Hl7ClientService : IHl7ClientService
{
    private readonly string _hisHost;
    private readonly int _hisPort;
    private readonly ILogger<Hl7ClientService> _logger;

    public Hl7ClientService(IConfiguration configuration, ILogger<Hl7ClientService> logger)
    {
        _hisHost = configuration["HIS:Host"] ?? "localhost";
        _hisPort = Convert.ToInt32(configuration["HIS:Port"] ?? "2575");
        _logger = logger;
    }

    public async Task SendHl7Message(string hl7Message)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(_hisHost, _hisPort);

        await using var stream = client.GetStream();
        var data = Encoding.UTF8.GetBytes(hl7Message);

        await stream.WriteAsync(data);
        await stream.FlushAsync();

        LogHl7MessageDetails(hl7Message);
    }

    private void LogHl7MessageDetails(string hl7Message)
    {
        _logger.LogInformation("Отправлено сообщение:");
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