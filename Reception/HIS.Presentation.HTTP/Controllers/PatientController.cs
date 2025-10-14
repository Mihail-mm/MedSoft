using HIS.Application.Contracts;
using HIS.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace HIS.Presentation.HTTP.Controllers;

[ApiController]
[Route("api/v1/patients")]
public class PatientController
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public IAsyncEnumerable<Patient> GetPatients()
    {
        return _patientService.GetAll();
    }
}