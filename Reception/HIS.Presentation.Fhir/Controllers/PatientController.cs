using HIS.Application.Contracts;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using DomainPatient = HIS.Application.Models.Patient;
using Patient = Hl7.Fhir.Model.Patient;

namespace HIS.Presentation.Fhir.Controllers;

[ApiController]
[Route("api/fhir/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientsService;

    public PatientController(IPatientService patientsService)
    {
        _patientsService = patientsService;
    }

    [HttpGet]
    [Produces("application/fhir+json")]
    public async Task<IActionResult> GetPatients()
    {
        var patients = await _patientsService.GetAll().ToListAsync();

        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Collection,
            Entry = patients.Select(p => new Bundle.EntryComponent { Resource = MapFromDomainPatient(p) }).ToList()
        };

        var fhirJson = new FhirJsonSerializer().SerializeToString(bundle);

        return Content(fhirJson, "application/fhir+json");
    }

    private static Patient MapFromDomainPatient(DomainPatient patient)
    {
        return new Patient
        {
            Id = patient.Id.ToString(),
            Name = [new HumanName { Family = patient.Surname, Given = [patient.Name] }],
            BirthDate = patient.BirthDate.ToString("yyyy-MM-dd"),
            Extension = [new Extension("http://example.org/fhir/StructureDefinition/patient-status", new FhirString("Arrived"))]
        };
    }
}