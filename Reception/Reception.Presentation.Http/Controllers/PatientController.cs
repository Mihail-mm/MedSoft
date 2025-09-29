using Microsoft.AspNetCore.Mvc;
using Reception.Application.Contracts;
using Reception.Application.Models;

namespace Reception.Presentation.Http.Controllers;

[ApiController]
[Route("api/v1/patient")]
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
    public async Task<IActionResult> GetPatient(long id)
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

    [HttpDelete]
    public async Task DeletePatient(long id)
    {
        await _patientService.DeletePatientById(id);
    }
}