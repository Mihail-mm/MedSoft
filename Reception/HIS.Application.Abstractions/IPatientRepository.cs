using HIS.Application.Models;

namespace HIS.Application.Abstractions;

public interface IPatientRepository
{
    IAsyncEnumerable<Patient> GetAll();
}