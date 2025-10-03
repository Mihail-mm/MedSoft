using Microsoft.AspNetCore.Mvc;
using Reception.Application.Contracts;
using Reception.Application.Models;

namespace Reception.Presentation.Http.Controllers;

[ApiController]
[Route("api/v1/patients")]
public class PatientController
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpPost]
    public async Task AddPatient(AddPatientRequest request)
    {
        await _patientService.AddPatient(request);
    }

    [HttpGet]
    public IAsyncEnumerable<Patient> GetPatients()
    {
        return _patientService.GetAllPatients();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetPatient([FromRoute] long id)
    {
        try
        {
            var patient = await _patientService.GetPatientById(id);
            return new OkObjectResult(patient);
        }
        catch
        {
            return new NotFoundResult();
        }
    }

    [HttpGet("by-request")]
    public IAsyncEnumerable<Patient> GetPatientBySearchRequest([FromQuery] SearchPatientRequest request)
    {
        return _patientService.GetPatientBySearchRequest(request);
    }

    [HttpDelete]
    public async Task DeletePatient(long id)
    {
        await _patientService.DeletePatientById(id);
    }
}