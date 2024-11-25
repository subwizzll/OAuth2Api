using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SchoolStaffAPI.Data;
using SchoolStaffAPI.Models;

namespace SchoolStaffAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UserController(SchoolStaffContext context) : ControllerBase
{
    [HttpPost("register")]
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
}