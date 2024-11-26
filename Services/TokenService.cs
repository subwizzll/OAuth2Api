using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SchoolStaffAPI.Data;
using SchoolStaffAPI.Models;
using Microsoft.EntityFrameworkCore;
using SchoolStaffAPI.Models.Entity;

namespace SchoolStaffAPI.Services;

public class TokenService(IConfiguration configuration, UserContext context)
{
    public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user, accessToken);
        
        return (accessToken, refreshToken.Token);
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var keyString = configuration.GetValue<string>("Jwt:Key") ??
            throw new InvalidOperationException("JWT Key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), // Short-lived access token
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(User user, string accessToken)
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiryDate = DateTime.UtcNow.AddHours(24),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            JwtId = jwtToken.Id
        };

        // Invalidate any existing refresh tokens for this user
        var existingTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == user.Id && 
                        !rt.Used && 
                        !rt.Invalidated && 
                        rt.RevokedAt == null && 
                        rt.ExpiryDate > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in existingTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.ReplacedByToken = refreshToken.Token;
        }

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || 
            storedToken.Used || 
            storedToken.Invalidated || 
            storedToken.RevokedAt != null || 
            storedToken.ExpiryDate <= DateTime.UtcNow || 
            storedToken.User == null)
            return null;

        // Generate new tokens
        var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(storedToken.User);

        // Invalidate the old refresh token
        storedToken.Used = true;
        storedToken.ReplacedByToken = newRefreshToken;
        await context.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken != null && 
            !storedToken.Used && 
            !storedToken.Invalidated && 
            storedToken.RevokedAt == null && 
            storedToken.ExpiryDate > DateTime.UtcNow)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }
} 