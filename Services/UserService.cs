using Microsoft.EntityFrameworkCore;
using SchoolStaffAPI.Data;
using SchoolStaffAPI.Models.Entity;
using SchoolStaffAPI.Services.Mixin;

namespace SchoolStaffAPI.Services;

public class UserService(UserContext context) : PasswordMixin
{
    public async Task RegisterUserAsync(User user)
    {
        user.Password = HashPassword(user.Password);
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public async Task<(string? ErrorMessage, User? User)> ValidateUserAsync(string email, string password)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return ("Email not found", null);

        if (!VerifyPassword(password, user.Password))
            return ("Invalid password", null);

        return (null, user);
    }
} 