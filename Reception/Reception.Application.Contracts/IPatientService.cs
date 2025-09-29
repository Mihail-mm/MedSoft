using Reception.Application.Models;

namespace Reception.Application.Contracts;

public interface IPatientService
{
    Task AddPatient(AddPatientRequest request);

    Task<Patient> GetPatientById(long id);
    
    Task DeletePatientById(long id);
}