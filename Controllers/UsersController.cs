using Microsoft.AspNetCore.Mvc;
using SchoolStaffApi.Data;
using SchoolStaffApi.Models;

namespace SchoolStaffApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(SchoolStaffContext context) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return Ok(new { message = "User registered successfully" });
        }
    }
} 