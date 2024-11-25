using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SchoolStaffAPI.Data;
using SchoolStaffAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SchoolStaffAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(SchoolStaffContext context, IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    [Authorize(AuthenticationSchemes = "Secret", Policy = "RequireSecret")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Hash the password
        var passwordBytes = SHA256.HashData(Encoding.UTF8.GetBytes(user.Password));
        var builder = new StringBuilder();

        foreach (var b in passwordBytes)
            builder.Append(b.ToString("x2"));

        user.Password = builder.ToString();

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        // Hash the provided password and compare
        var hashedPassword = Convert.ToHexString(
SHA256.HashData(Encoding.UTF8.GetBytes(request.Password))).ToLower();

        if (user.Password != hashedPassword)
            return Unauthorized(new { message = "Invalid username or password" });

        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var keyString = configuration.GetValue<string>("Jwt:Key") ?? throw new InvalidOperationException("JWT Key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var responseToken = new JwtSecurityTokenHandler().WriteToken(token);

        return responseToken;
    }
}