using Reception.Application.Abstraction;
using Reception.Application.Contracts;
using Reception.Application.Models;

namespace Reception.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IHl7ClientService _hl7ClientService;

    public PatientService(IPatientRepository patientRepository, IHl7ClientService hl7ClientService)
    {
        _patientRepository = patientRepository;
        _hl7ClientService = hl7ClientService;
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
}