using HIS.Application.Contracts;
using HIS.Application.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;
using DomainPatient = HIS.Application.Models.Patient;
using Patient = Hl7.Fhir.Model.Patient;

namespace HIS.Presentation.Fhir.Services;

public class FhirPatientService : IFhirPatientService
{
    private readonly IPatientService _patientsService;
    private readonly FhirJsonSerializer _serializer;
    private readonly FhirClient _fhirClient;
    private readonly ILogger<FhirPatientService> _logger;

    private static FhirClientSettings Settings = new()
    {
        PreferredFormat = ResourceFormat.Json
    };

    public FhirPatientService(IPatientService patientsService, ILogger<FhirPatientService> logger)
    {
        _patientsService = patientsService;
        _serializer = new FhirJsonSerializer();
        _fhirClient = new FhirClient("https://localhost:7066/api/fhir", Settings);
        _logger = logger;
    }

    public async Task<string> GetPatients()
    {
        var patients = await _patientsService.GetAll().ToListAsync();

        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Collection,
            Entry = patients.Select(p => new Bundle.EntryComponent { Resource = MapFromDomainPatient(p) }).ToList()
        };
        
        var message = _serializer.SerializeToString(bundle);
        _logger.LogInformation(message);
        return message;
    }

    public async Task<string> GetPatient(long id)
    {
        var patient = await _patientsService.GetPatientById(id);
        var message = _serializer.SerializeToString(MapFromDomainPatient(patient));
        _logger.LogInformation(message);
        return message;
    }

    public async Task PutPatient(long id, Patient patient)
    {
        var domainPatient = mapFromHl7Patient(patient);
        await _patientsService.PatchPatientStatus(id, domainPatient.Status);
        await _fhirClient.UpdateAsync(patient);
    }
    
    private static Patient MapFromDomainPatient(DomainPatient patient)
    {
        return new Patient
        {
            Id = patient.Id.ToString(),
            Name = [new HumanName { Family = patient.Surname, Given = [patient.Name] }],
            BirthDate = patient.BirthDate.ToString("yyyy-MM-dd"),
            Extension =
            [
                new Extension("http://example.org/fhir/StructureDefinition/patient-status", new FhirString(patient.Status.ToString())),
            ]
        };
    }

    private static DomainPatient mapFromHl7Patient(Patient patient)
    {
        return new DomainPatient(
            Id: long.Parse(patient.Id),
            Name: patient.Name.FirstOrDefault()?.Given.FirstOrDefault() ?? "",
            Surname: patient.Name.FirstOrDefault()?.Family ?? "",
            BirthDate: DateOnly.Parse(patient.BirthDate),
            Status: Enum.Parse<PatientStatus>(
                patient.Extension.FirstOrDefault()?.Value.ToString() ?? "Arrived", true));
    }
}