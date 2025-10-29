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

    public async Task<Patient> GetPatientById(long id)
    {
        return await _patientRepository.GetPatientById(id);
    }

    public IAsyncEnumerable<Patient> GetAll()
    {
        return _patientRepository.GetAll();
    }

    public async Task DeletePatient(long patientId)
    {
        await _patientRepository.DeletePatient(patientId);
    }

    public async Task PatchPatientStatus(long patientId, PatientStatus status)
    {
        await _patientRepository.PatchPatientStatus(patientId, status);
    }
}