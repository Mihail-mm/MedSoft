using Reception.Application.Models;

namespace Reception.Application.Contracts;

public interface IPatientService
{
    Task AddPatient(AddPatientRequest request);

    IAsyncEnumerable<Patient> GetAllPatients();

    Task<Patient> GetPatientById(long id);

    IAsyncEnumerable<Patient> GetPatientBySearchRequest(SearchPatientRequest request);

    Task DeletePatientById(long id);

    Task PatchStatus(long id);
}