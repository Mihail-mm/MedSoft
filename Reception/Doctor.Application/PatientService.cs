using Doctor.Contracts;
using Doctor.Models;
using Hl7.Fhir.Rest;
using DomainPatient = Doctor.Models.Patient;
using Patient = Hl7.Fhir.Model.Patient;

namespace Doctor.Application;

public class PatientService : IPatientService
{
    private readonly FhirClient _fhirClient;

    public PatientService()
    {
        _fhirClient = new FhirClient("https://localhost:7226/api/fhir");
    }

    public async Task<IEnumerable<DomainPatient>> GetPatients()
    {
        var bundle = await _fhirClient.SearchAsync<Patient>();
        return bundle.Entry
            .Select(e => (Patient)e.Resource)
            .Select(p => new DomainPatient(
                Id: long.Parse(p.Id),
                Name: p.Name.FirstOrDefault()?.Given.FirstOrDefault() ?? "",
                Surname: p.Name.FirstOrDefault()?.Family ?? "",
                BirthDate: DateOnly.Parse(p.BirthDate),
                Status: Enum.Parse<PatientStatus>(
                    p.Extension.FirstOrDefault()?.Value.ToString() ?? "Arrived", true)
            ));
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