using Microsoft.EntityFrameworkCore;
using OAuth2Api.Data;
using OAuth2Api.Models.Entity;
using OAuth2Api.Services.Mixin;

namespace OAuth2Api.Services;

public class UserService(UserContext context) : PasswordMixin
{
    public async Task<(string? ErrorMessage, User? User)> RegisterUserAsync(User user)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (existingUser != null)
            return ("Email already registered", null);

        user.Password = HashPassword(user.Password);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        return (null, user);
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