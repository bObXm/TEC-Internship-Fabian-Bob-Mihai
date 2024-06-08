using Internship.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Internship.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalariesController : ControllerBase
    {
        private readonly ILogger<SalariesController> _logger;
        public SalariesController(ILogger<SalariesController> logger)
        {
            this._logger = logger;
        }

        [HttpGet, Authorize]
        public IActionResult Get()
        {
            var db = new APIDbContext();
            var list = db.Salaries.ToList();
            return Ok(list);
        }

        [HttpDelete("{Id}"), Authorize]
        public IActionResult Delete(int Id)
        {
            var db = new APIDbContext();
            Salary salary = db.Salaries.Find(Id);
            if (salary == null)
                return NotFound();
            else
            {
                db.Salaries.Remove(salary);
                db.SaveChanges();
                return NoContent();
            }
        }

        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Salary salary)
        {
            if (salary == null)
            {
                return BadRequest("Add a salary.");
            }
            try
            {
                var db = new APIDbContext();
                db.Salaries.Add(salary);
                db.SaveChanges();
                return StatusCode(201, salary);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding a salary");
                return StatusCode(500, "An error occurred while saving the salary.");
            }
        }
    }
}
