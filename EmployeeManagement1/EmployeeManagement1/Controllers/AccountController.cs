using EmployeeManagement.Models;
using EmployeeManagement.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Login model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountRepository.RegisterAsync(model);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _accountRepository.LoginAsync(model);
            if (token == null)
                return Unauthorized(); 

            return Ok(new { Token = token });
        }
    }
}
        