using Microsoft.AspNetCore.Mvc;
using VMTask.Data;
using VMTask.Models;


namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PeopleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetPeople()
        {
            var people = _context.People.ToList();
            return Ok(people);
        }

        [HttpPost]
        public IActionResult AddPerson([FromBody] Person person)
        {
            _context.People.Add(person);
            _context.SaveChanges();
            return Ok(person);
        }
    }
}
