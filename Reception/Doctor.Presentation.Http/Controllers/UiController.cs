using Doctor.Contracts;
using Doctor.Models;
using Doctor.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.Presentation.Http.Controllers;

[ApiController]
[Route("api/v1/patients")]
public class UiController : ControllerBase
{
    private readonly IPatientService _patientService;

    public UiController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IEnumerable<Patient>> GetPatients()
    {
        return await _patientService.GetPatients();
    }

    [HttpPatch("{patientId:long}/start")]
    public async Task StartAppointment([FromRoute] long patientId)
    {
        await _patientService.StartAppointment(patientId);
    }

    [HttpPatch("{patientId:long}/finish")]
    public async Task<IResult> FinishAppointment([FromRoute] long patientId)
    {
        try
        {
            await _patientService.FinishAppointment(patientId);
            return Results.Ok();
        }
        catch (ConflictStatusException ex)
        {
            return Results.Conflict(ex.Message);
        }
    }
}