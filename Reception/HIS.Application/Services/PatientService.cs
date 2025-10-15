using HIS.Application.Abstractions;
using HIS.Application.Contracts;
using HIS.Application.Models;

namespace HIS.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task AddPatient(Patient patient)
    {
        await _patientRepository.AddPatient(patient);
    }

    public IAsyncEnumerable<Patient> GetAll()
    {
        return _patientRepository.GetAll();
    }

    public async Task DeletePatient(long patientId)
    {
        await _patientRepository.DeletePatient(patientId);
    }
}