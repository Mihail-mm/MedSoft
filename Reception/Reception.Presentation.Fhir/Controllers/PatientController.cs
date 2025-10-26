using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using Reception.Application.Contracts;
using Reception.Application.Models;
using Patient = Hl7.Fhir.Model.Patient;
using DomainPatient = Reception.Application.Models.Patient;

namespace Reception.Presentation.Fhir.Controllers;

[ApiController]
[Route("api/fhir/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly FhirJsonParser _parser = new();

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpPut("{id:long}")]
    [Consumes("application/fhir+json")]
    [Produces("application/fhir+json")]
    public async Task<IActionResult> PutPatient([FromRoute] long id)
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        var patient = _parser.Parse<Patient>(body);
        var domainPatient = mapFromHl7Patient(patient);
        await _patientService.PatchStatus(id, domainPatient.Status);
        return Ok();
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