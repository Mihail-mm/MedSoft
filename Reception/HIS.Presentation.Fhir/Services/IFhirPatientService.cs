using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace HIS.Presentation.Fhir.Services;

public interface IFhirPatientService
{
    Task<string> GetPatients();
    Task<string> GetPatient(long id);
    Task PutPatient(long id, Patient patient);
}