using Internship.Model;
using Internship.ObjectModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace Internship.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly ILogger<PersonsController> _logger;
        public PersonsController(ILogger<PersonsController> logger)
        {
            this._logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var db = new APIDbContext();
            var list = db.Persons.Include(x => x.Salary).Include(x => x.Position)
                   .Select(x => new PersonInformation()
                   {
                       Id = x.Id,
                       Name = x.Name,
                       PositionName = x.Position.Name,
                       Salary = x.Salary.Amount,
                   }).ToList();
            return Ok(list);
        }

        [HttpGet("{Id}")]
        public IActionResult Get(int Id)
        {
            var db = new APIDbContext();
            Person person = db.Persons.FirstOrDefault(x => x.Id == Id);
            if (person == null)
                return NotFound();
            else
                return Ok(person);

        }

        [HttpPost]
        public IActionResult Post(Person person)
        {
            if (ModelState.IsValid)
            {
                var db = new APIDbContext();
                db.Persons.Add(person);
                db.SaveChanges();
                return Created("", person);
            }
            else
                return BadRequest();
        }

        [HttpPut]
        public IActionResult UpdatePerson([FromBody] Person person)
        {

            if (ModelState.IsValid)
            {
                var db = new APIDbContext();

                var positionExists = db.Positions.Any(p => p.PositionId == person.PositionId);
                if (!positionExists)
                {
                    ModelState.AddModelError("PositionId", "Invalid PositionId. Position does not exist.");
                    return BadRequest(ModelState);
                }

                
                var salaryExists = db.Salaries.Any(s => s.SalaryId == person.SalaryId);
                if (!salaryExists)
                {
                    ModelState.AddModelError("SalaryId", "Invalid SalaryId. Salary does not exist.");
                    return BadRequest(ModelState);
                }

                Person updateperson = db.Persons.Find(person.Id);

                if (updateperson == null)
                {
                    _logger.LogWarning($"Person with ID {person.Id} not found.");
                    return NotFound("Person not found");
                }

                updateperson.Address = person.Address;
                updateperson.Age = person.Age;
                updateperson.Email = person.Email;
                updateperson.Name = person.Name;
                updateperson.PositionId = person.PositionId;
                updateperson.SalaryId = person.SalaryId;
                updateperson.Surname = person.Surname;
                db.SaveChanges();
                return NoContent();
            }
            else{
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeletePerson(int id)
        {
            try
            {
                var db = new APIDbContext();
                var person = await db.Persons.FindAsync(id);
                if (person == null)
                {
                    return NotFound("Person not found");
                }
                db.Persons.Remove(person);
                await db.SaveChangesAsync();
                return Ok("User deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting person");
                return StatusCode(500, "An error occurred while deleting the user");
            }
        }
    }
}
