using HIS.Application.Models;

namespace HIS.Application.Abstractions;

public interface IPatientRepository
{
    Task AddPatient(Patient patient);
    IAsyncEnumerable<Patient> GetAll();
    Task DeletePatient(long patientId);
    Task PatchPatientStatus(long patientId, PatientStatus status);
}