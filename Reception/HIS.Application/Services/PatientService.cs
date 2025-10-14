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

    public IAsyncEnumerable<Patient> GetAll()
    {
        return _patientRepository.GetAll();
    }
}