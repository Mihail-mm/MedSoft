using HIS.Application.Models;

namespace HIS.Application.Contracts;

public interface IPatientService
{
    IAsyncEnumerable<Patient> GetAll();
}