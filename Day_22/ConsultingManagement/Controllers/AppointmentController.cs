using Microsoft.AspNetCore.Mvc;
using ConsultingManagement.Models.DTOs;
using ConsultingManagement.Interfaces;
using ConsultingManagement.Models;
using Microsoft.AspNetCore.Authorization;


namespace ConsultingManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment([FromBody] AppointmentAddRequestDto appointment)
        {
            try
            {
                var newAppointment = await _appointmentService.AddAppointment(appointment);
                if (newAppointment != null)
                    return Created("", newAppointment);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Doctor")]
        [Authorize(Policy = "AppointmentCancellation")]
        public async Task<ActionResult<Appointment>> CancelAppointment(string AppointmentNumber)
        {
            var appointment = await _appointmentService.CancelAppointment(AppointmentNumber);
            return Ok(appointment);
        }
    }

    
}