using Doctor.Models;

namespace Doctor.Contracts;

public interface IPatientService
{
    Task<List<Patient>> GetPatients();

    Task StartAppointment(long patientId);

    Task FinishAppointment(long patientId);
}