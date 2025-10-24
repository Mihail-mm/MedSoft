using HIS.Application.Models;

namespace HIS.Application.Contracts;

public interface IPatientService
{
    Task AddPatient(Patient patient);
    IAsyncEnumerable<Patient> GetAll();
    Task DeletePatient(long patientId);
    Task PatchPatientStatus(long patientId, PatientStatus status);
}