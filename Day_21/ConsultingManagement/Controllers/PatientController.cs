using Microsoft.AspNetCore.Mvc;
using ConsultingManagement.Models.DTOs;
using ConsultingManagement.Interfaces;
using ConsultingManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace ConsultingManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient([FromBody] PatientAddRequestDto patient)
        {
            try
            {
                var newPatient = await _patientService.AddPatient(patient);
                if (newPatient != null)
                    return Created("", newPatient);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}