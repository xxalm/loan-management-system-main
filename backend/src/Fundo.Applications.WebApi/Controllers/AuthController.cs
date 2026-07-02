using Fundo.Applications.WebApi.Models.Requests;
using Fundo.Applications.WebApi.Models.Responses;
using Fundo.Applications.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            if (request.Username != "admin" || request.Password != "admin")
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var token = _jwtTokenService.GenerateToken(request.Username);

            return Ok(new LoginResponse { Token = token });
        }
    }
}
