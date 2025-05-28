using Microsoft.AspNetCore.Mvc;
using ConsultingManagement.Models;

namespace ConsultingManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private static List<Doctor> _doctors = new List<Doctor>();


        [HttpGet]
        public ActionResult<IEnumerable<Doctor>> GetDoctors()
        {
            if (_doctors.Count > 0)
            {
                return Ok(_doctors);
            }
            return NotFound();
        }

        [HttpGet("{id}")]
        public ActionResult<Doctor> GetDoctorById(int id)
        {
            Doctor? doctor = _doctors.FirstOrDefault(d => d.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }

        [HttpPost]
        public ActionResult<Doctor> PostDoctor([FromBody] Doctor d)
        {
            _doctors.Add(d);
            return Created($"/api/doctor/{d.Id}", d);
        }

        [HttpDelete("{id}")]
        public ActionResult RemoveDoctor(int id)
        {
            Doctor? doctor = _doctors.FirstOrDefault(d => d.Id == id);
            if (doctor == null)
            {
                return BadRequest();
            }
            _doctors.Remove(doctor);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateDoctor(int id, [FromBody] Doctor doctor)
        {
            Doctor? doc = _doctors.FirstOrDefault(d => d.Id == id);
            if (doc == null)
            {
                return NotFound();
            }
            doc.Name = doctor.Name;
            return NoContent();
        }
    }
}