using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Reception.Application.Abstraction;
using Reception.Application.Contracts;
using Reception.Application.Models;
using Reception.Application.Models.Exceptions;
using FhirPatient = Hl7.Fhir.Model.Patient;
using Patient = Reception.Application.Models.Patient;
using Task = System.Threading.Tasks.Task;

namespace Reception.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IHl7ClientService _hl7ClientService;
    private readonly FhirClient _fhirClient;

    private static FhirClientSettings Settings = new()
    {
        PreferredFormat = ResourceFormat.Json
    };

    public PatientService(IPatientRepository patientRepository, IHl7ClientService hl7ClientService)
    {
        _patientRepository = patientRepository;
        _hl7ClientService = hl7ClientService;
        _fhirClient = new FhirClient("https://localhost:7226/api/fhir", Settings);
    }

    public IAsyncEnumerable<Patient> GetAllPatients()
    {
        return _patientRepository.GetAllPatients();
    }

    public async Task AddPatient(AddPatientRequest request)
    {
        var patientId = await _patientRepository.AddPatient(request);
        var patientHl7 = new PatientHl7Model(patientId, request.Name, request.Surname, request.DateOfBirth, "A01");
        var hl7Message = Hl7MessageService.CreateAdtMessage(patientHl7);
        await _hl7ClientService.SendHl7Message(hl7Message);
    }

    public async Task<Patient> GetPatientById(long id)
    {
        return await _patientRepository.GetPatientById(id);
    }

    public IAsyncEnumerable<Patient> GetPatientBySearchRequest(SearchPatientRequest request)
    {
        return _patientRepository.GetPatientBySearchRequest(request);
    }

    public async Task DeletePatientById(long id)
    {
        var patient = await _patientRepository.GetPatientById(id);
        var patientHl7 = new PatientHl7Model(patient.Id, patient.Name, patient.Surname, patient.BirthDate, "A03");
        var hl7Message = Hl7MessageService.CreateAdtMessage(patientHl7);
        await _hl7ClientService.SendHl7Message(hl7Message);
        await _patientRepository.DeletePatientById(id);
    }

    public async Task PatchStatus(long id, PatientStatus status)
    {
        await _patientRepository.PatchPatientStatus(id, status);
    }

    public async Task PatientArrived(long id)
    {
        await ValidatePatientBeforePatchStatus(id);
        await PatchStatus(id, PatientStatus.Arrived);

        var patient = await _fhirClient.ReadAsync<FhirPatient>($"Patient/{id}");

        var statusExtensionUrl = "http://example.org/fhir/StructureDefinition/patient-status";
        var existing = patient?.Extension.FirstOrDefault(e => e.Url == statusExtensionUrl);
        if (existing != null)
        {
            existing.Value = new FhirString(PatientStatus.Arrived.ToString());
        }
        else
        {
            patient.Extension.Add(new Extension(statusExtensionUrl,
                new FhirString(PatientStatus.Arrived.ToString())));
        }

        await _fhirClient.UpdateAsync(patient);
    }

    private async Task ValidatePatientBeforePatchStatus(long id)
    {
        var receptionPatient = await _patientRepository.GetPatientById(id);

        if (receptionPatient.Status is PatientStatus.Started)
        {
            throw new ConflictException("The patient is already at the doctor's");
        }
    }
}