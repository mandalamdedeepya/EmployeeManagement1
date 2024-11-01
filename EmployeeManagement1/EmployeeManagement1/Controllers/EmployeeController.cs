using EmployeeManagement.Models;
using EmployeeManagement.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? filter = null, string? sort = null, int page = 1, int pageSize = 10)
        {
            var result = await _repository.GetAllAsync(filter, sort, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpGet("byids")]
        public async Task<IActionResult> GetByIds([FromQuery] List<int> ids, string? filter = null, string? sort = null, int page = 1, int pageSize = 10)
        {
            var result = await _repository.GetByIdsAsync(ids, filter, sort, page, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (await _repository.IsEmployeeExists(employee.Email))
            {
                return Conflict("Employee already exists");
            }

            var employeeId = await _repository.CreateAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (id <= 0)
            {
                return BadRequest($"Employee ID {id} must be a positive number");
            }

            var existingEmployee = await _repository.GetByIdAsync(id);

            if (existingEmployee == null)
            {
                return NotFound($"Employee ID {id} does not exist");
            }

            var updatedEmployee = await _repository.UpdateAsync(employee, existingEmployee);
            return Ok("Updated Employee");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (id <= 0)
            {
                return BadRequest($"Employee ID {id} must be a positive number");
            }

            var existingEmployee = await _repository.GetByIdAsync(id);

            if (existingEmployee == null)
            {
                return NotFound("Employee Id not found");
            }

            await _repository.DeleteAsync(id);
            return Ok("Deleted Successfully");
        }
    }
}
