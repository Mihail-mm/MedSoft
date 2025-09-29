using Reception.Application.Models;

namespace Reception.Application.Abstraction;

public interface IPatientRepository
{
    Task AddPatient(AddPatientRequest request);
    
    Task<Patient> GetPatientById(long id);
    
    Task DeletePatientById(long id);
}