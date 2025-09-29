using Reception.Application.Abstraction;
using Reception.Application.Contracts;
using Reception.Application.Models;

namespace Reception.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task AddPatient(AddPatientRequest request)
    {
        await _patientRepository.AddPatient(request);
    }

    public async Task<Patient> GetPatientById(long id)
    {
        return await _patientRepository.GetPatientById(id);
    }

    public async Task DeletePatientById(long id)
    {
        await _patientRepository.DeletePatientById(id);
    }
}