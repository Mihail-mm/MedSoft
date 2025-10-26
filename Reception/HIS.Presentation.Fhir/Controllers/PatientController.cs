using HIS.Presentation.Fhir.Services;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using Patient = Hl7.Fhir.Model.Patient;

namespace HIS.Presentation.Fhir.Controllers;

[ApiController]
[Route("api/fhir/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IFhirPatientService _fhirPatientService;
    private readonly FhirJsonParser _parser = new();

    public PatientController(IFhirPatientService fhirPatientService)
    {
        _fhirPatientService = fhirPatientService;
    }

    [HttpGet("{id:long}")]
    [Produces("application/fhir+json")]
    public async Task<IActionResult> GetById(long id)
    {
        var patientJson = await _fhirPatientService.GetPatient(id);
        return Content(patientJson, "application/fhir+json");
    }

    [HttpGet]
    [Produces("application/fhir+json")]
    public async Task<IActionResult> GetPatients()
    {
        var fhirJson = await _fhirPatientService.GetPatients();
        return Content(fhirJson, "application/fhir+json");
    }

    [HttpPut("{id:long}")]
    [Consumes("application/fhir+json")]
    [Produces("application/fhir+json")]
    public async Task<IActionResult> PutPatient([FromRoute] long id)
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        var patient = _parser.Parse<Patient>(body);

        await _fhirPatientService.PutPatient(id, patient);
        return Ok();
    }
}