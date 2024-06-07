using Internship.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;


namespace Internship.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonDetailsController : ControllerBase
    {
        private readonly ILogger<PersonDetailsController> _logger;

        public PersonDetailsController(ILogger<PersonDetailsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PersonDetail personDetail)
        {
            if (personDetail == null)
            {
                return BadRequest("PersonDetail is null.");
            }

            try
            {
                var db = new APIDbContext();
                db.PersonDetails.Add(personDetail);
                await db.SaveChangesAsync();
                return StatusCode(201, personDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding person detail.");
                return StatusCode(500, "An error occurred while adding the person detail.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var db = new APIDbContext();
                {
                    var personDetail = db.PersonDetails
                                .Include(pd => pd.Person)
                                .FirstOrDefault(pd => pd.Id == id);

                    if (personDetail == null)
                    {
                        _logger.LogWarning("PersonDetail with ID {Id} not found.", id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogInformation("PersonDetail retrieved successfully.");
                        return Ok(personDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching person detail.");
                return StatusCode(500, "An error occurred while fetching the person detail.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PersonDetail personDetail)
        {
            if (personDetail == null || personDetail.Id != id)
            {
                return BadRequest("PersonDetail is null or ID mismatch.");
            }

            try
            {
                var db = new APIDbContext();
                var existingPersonDetail = db.PersonDetails
                                                  .Include(pd => pd.Person)
                                                  .FirstOrDefault(pd => pd.Id == id);

                if (existingPersonDetail == null)
                {
                    _logger.LogWarning("PersonDetail with ID {Id} not found.", id);
                    return NotFound();
                }

                
                if (existingPersonDetail.Person == null)
                {
                    existingPersonDetail.Person = new Person();
                }

              
                existingPersonDetail.BirthDay = personDetail.BirthDay;
                existingPersonDetail.PersonCity = personDetail.PersonCity;
                existingPersonDetail.Person.Name = personDetail.Person.Name;
                existingPersonDetail.Person.Surname = personDetail.Person.Surname;
                existingPersonDetail.Person.Email = personDetail.Person.Email;
                existingPersonDetail.Person.Address = personDetail.Person.Address;
                existingPersonDetail.Person.PositionId = personDetail.Person.PositionId;
                existingPersonDetail.Person.SalaryId = personDetail.Person.SalaryId;

                await db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating person detail.");
                return StatusCode(500, "An error occurred while updating the person detail.");
            }
        }
    }
}

