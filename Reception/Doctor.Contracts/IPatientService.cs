using Doctor.Models;

namespace Doctor.Contracts;

public interface IPatientService
{
    Task<IEnumerable<Patient>> GetPatients();

    Task StartAppointment(long patientId);

    Task FinishAppointment(long patientId);
}