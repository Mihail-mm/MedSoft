using Reception.Application.Models;

namespace Reception.Application.Abstraction;

public interface IPatientRepository
{
    Task<long> AddPatient(AddPatientRequest request);

    IAsyncEnumerable<Patient> GetAllPatients();

    Task<Patient> GetPatientById(long id);

    IAsyncEnumerable<Patient> GetPatientBySearchRequest(SearchPatientRequest request);

    Task DeletePatientById(long id);

    Task PatchPatientStatus(long id, PatientStatus status);
}