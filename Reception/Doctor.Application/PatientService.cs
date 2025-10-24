using Doctor.Contracts;
using Doctor.Models;

namespace Doctor.Application;

public class PatientService : IPatientService
{
    public Task<List<Patient>> GetPatients()
    {
        throw new NotImplementedException();
    }

    public Task StartAppointment(long patientId)
    {
        throw new NotImplementedException();
    }

    public Task FinishAppointment(long patientId)
    {
        throw new NotImplementedException();
    }
}