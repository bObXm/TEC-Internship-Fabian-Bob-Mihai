using Internship.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internship.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(ILogger<DepartmentsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var db = new APIDbContext();
            var list = db.Departments.ToList();
            return Ok(list);
        }

        [HttpGet("{Id}")]
        public IActionResult Get(int Id)
        {
            var db = new APIDbContext();
            Department department = db.Departments.Find(Id);
            if (department == null)
                return NotFound();
            else
                return Ok(department);
        }

        [HttpPost]
        public IActionResult AddDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                var db = new APIDbContext();
                db.Departments.Add(department);
                db.SaveChanges();
                return Created("", department);
            }
            else
                return BadRequest();

        }
        [HttpPut]
        public IActionResult UpdateDepartment(Department department)
        {

            if (ModelState.IsValid)
            {
                var db = new APIDbContext();
                Department updateDepartment = db.Departments.Find(department.DepartmentId);
                updateDepartment.DepartmentName = department.DepartmentName;
                db.SaveChanges();
                return NoContent();
            }
            else
                return BadRequest();
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var db = new APIDbContext();
            try
            {
                var department = await db.Departments.FindAsync(Id);
                if (department == null)
                {
                    _logger.LogWarning($"Department with Id = {Id} not found.");
                    return NotFound();
                }

                db.Departments.Remove(department);
                await db.SaveChangesAsync();

                _logger.LogInformation($"Department with Id = {Id} deleted successfully.");
                return NoContent();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting department with Id = {Id}");
                return StatusCode(500, "An error occurred while deleting the department.");
            }
        }
    }
}
