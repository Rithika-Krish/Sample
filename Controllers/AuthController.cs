using Microsoft.AspNetCore.Mvc;
using SampleAuth_WebAPI.DTO;
using SampleAuth_WebAPI.Services;

namespace SampleAuth_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // ---------------- REGISTER ----------------
        [HttpPost("register")]
        public IActionResult Register(RegisterDTO dto)
        {
            var result = _authService.Register(dto);

            if (result == "User already exists")
                return BadRequest(result);

            return Ok(result);
        }

        // ---------------- LOGIN ----------------
        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {
            var token = _authService.Login(dto);

            if (token == null)
                return Unauthorized("Invalid credentials");

            return Ok(new { token });
        }
    }
}