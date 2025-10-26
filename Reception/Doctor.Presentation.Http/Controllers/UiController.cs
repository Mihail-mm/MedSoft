using Doctor.Contracts;
using Doctor.Models;
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
    public async Task FinishAppointment([FromRoute] long patientId)
    {
        await _patientService.FinishAppointment(patientId);
    }
}