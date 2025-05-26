using FirstAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        static List<Patient> _patients = new List<Patient>();

        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetAllPatients()
        {
            if (_patients.Count > 0)
            {
                return Ok(_patients);
            }
            return NotFound();
        }

        [HttpGet("{id}")]
        public ActionResult<Patient> GetPatientById(int id)
        {
            Patient? patient = _patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpPost]
        public ActionResult<Patient> AddPatient([FromBody] Patient p)
        {
            _patients.Add(p);
            return Created($"/api/patient/{p.Id}", p);
        }

        [HttpDelete("{id}")]
        public ActionResult RemovePatient(int id)
        {
            Patient? patient = _patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound();
            }
            _patients.Remove(patient);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult UpdatePatient(int id, [FromBody] Patient p)
        {
            Patient? patient = _patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound();
            }
            patient.Name = p.Name;
            patient.Age = p.Age;
            return NoContent();
        }
    }
}