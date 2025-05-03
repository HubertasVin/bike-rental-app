using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BikeRentalApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public UserController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User request)
        {
            if (request.Name == "admin" && request.PasswordHash == "password")
            {
                var token = _jwtService.GenerateJwtToken(request.Name);
                return Ok(new { token });
            }
            return Unauthorized();
        }
    }
}
