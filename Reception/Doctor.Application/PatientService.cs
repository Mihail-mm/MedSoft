using Doctor.Contracts;
using Doctor.Models;
using Doctor.Models.Exceptions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using DomainPatient = Doctor.Models.Patient;
using Patient = Hl7.Fhir.Model.Patient;
using Task = System.Threading.Tasks.Task;

namespace Doctor.Application;

public class PatientService : IPatientService
{
    private readonly FhirClient _fhirClient;

    private static FhirClientSettings Settings = new()
    {
        PreferredFormat = ResourceFormat.Json
    };

    public PatientService()
    {
        _fhirClient = new FhirClient("https://localhost:7226/api/fhir", Settings);
    }

    public async Task<IEnumerable<DomainPatient>> GetPatients()
    {
        var bundle = await _fhirClient.SearchAsync<Patient>();
        var patients = bundle.Entry
            .Select(e => (Patient)e.Resource)
            .Select(p => new DomainPatient(
                Id: long.Parse(p.Id),
                Name: p.Name.FirstOrDefault()?.Given.FirstOrDefault() ?? "",
                Surname: p.Name.FirstOrDefault()?.Family ?? "",
                BirthDate: DateOnly.Parse(p.BirthDate),
                Status: Enum.Parse<PatientStatus>(
                    p.Extension.FirstOrDefault()?.Value.ToString() ?? "Arrived", true)
            ));

        return patients.Where(patient => patient.Status is PatientStatus.Arrived or PatientStatus.Started);
    }

    public async Task StartAppointment(long patientId)
    {
        var patient = await _fhirClient.ReadAsync<Patient>($"Patient/{patientId}");

        var statusExtensionUrl = "http://example.org/fhir/StructureDefinition/patient-status";
        var existing = patient.Extension.FirstOrDefault(e => e.Url == statusExtensionUrl);
        if (existing != null)
        {
            existing.Value = new FhirString(PatientStatus.Started.ToString());
        }
        else
        {
            patient.Extension.Add(new Extension(statusExtensionUrl, new FhirString(PatientStatus.Started.ToString())));
        }

        await _fhirClient.UpdateAsync(patient);
    }

    public async Task FinishAppointment(long patientId)
    {
        var patient = await _fhirClient.ReadAsync<Patient>($"Patient/{patientId}");
        ValidateBeforePatchStatus(patient);

        var statusExtensionUrl = "http://example.org/fhir/StructureDefinition/patient-status";
        var existing = patient.Extension.FirstOrDefault(e => e.Url == statusExtensionUrl);
        if (existing != null)
        {
            existing.Value = new FhirString(PatientStatus.Completed.ToString());
        }
        else
        {
            patient.Extension.Add(new Extension(statusExtensionUrl,
                new FhirString(PatientStatus.Completed.ToString())));
        }

        await _fhirClient.UpdateAsync(patient);
    }

    private void ValidateBeforePatchStatus(Patient patient)
    {
        var domainPatient = mapFromHl7Patient(patient);
        if (domainPatient.Status is not PatientStatus.Started)
        {
            throw new ConflictStatusException("Patient status is invalid for this patient");
        }
    }

    private static DomainPatient mapFromHl7Patient(Patient patient)
    {
        return new DomainPatient(
            Id: long.Parse(patient.Id),
            Name: patient.Name.FirstOrDefault()?.Given.FirstOrDefault() ?? "",
            Surname: patient.Name.FirstOrDefault()?.Family ?? "",
            BirthDate: DateOnly.Parse(patient.BirthDate),
            Status: Enum.Parse<PatientStatus>(
                patient.Extension.FirstOrDefault()?.Value.ToString() ?? "Arrived", true));
    }
}