using Leave_Managment_Sytem.Models;
using Leave_Managment_Sytem.Services;
using Microsoft.AspNetCore.Mvc;

namespace Leave_Managment_Sytem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Models.RegisterRequest request)
        {
            try
            {
                var userId = await _authService.RegisterAsync(request);

                return Ok(new
                {
                    message = "User registered successfully",
                    userId = userId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.LoginAsync(request);

                return Ok(new
                {
                    message = "Login successful",
                    token = token
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
