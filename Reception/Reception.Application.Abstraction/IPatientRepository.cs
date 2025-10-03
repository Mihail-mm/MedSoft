using Reception.Application.Models;

namespace Reception.Application.Abstraction;

public interface IPatientRepository
{
    Task AddPatient(AddPatientRequest request);

    IAsyncEnumerable<Patient> GetAllPatients();

    Task<Patient> GetPatientById(long id);

    IAsyncEnumerable<Patient> GetPatientBySearchRequest(SearchPatientRequest request);

    Task DeletePatientById(long id);
}