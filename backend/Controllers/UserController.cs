using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Data;
using BikeRentalApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;
    private readonly JwtService _jwtService;

    public UserController(ApplicationDbContext db, IConfiguration config, JwtService jwtService)
    {
        _db = db;
        _config = config;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO request)
    {
        if (_db.Users.Any(u => u.Email == request.Email))
            return BadRequest("Email already exists");

        var user = new User(request.Name, request.Email, request.Password);


        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(user);
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginDTO request)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = _jwtService.GenerateJwtToken(user);
        return Ok(new AuthDTO(token));
    }

}
