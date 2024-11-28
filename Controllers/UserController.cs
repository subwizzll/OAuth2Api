using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OAuth2Api.Models.Entity;
using OAuth2Api.Services;

namespace OAuth2Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(TokenService tokenService, UserService userService) : ControllerBase
{
    [HttpPost("register")]
    [Authorize(AuthenticationSchemes = "Secret", Policy = "RequireSecret")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (errorMessage, _) = await userService.RegisterUserAsync(user);

        if (errorMessage != null)
            return BadRequest(new { message = errorMessage });

        return Ok(new { message = "User registered successfully", user});
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] string email, [FromBody] string password)
    {
        var (errorMessage, user) = await userService.ValidateUserAsync(email, password);

        if (errorMessage != null)
            return Unauthorized(new { message = errorMessage });

        var (accessToken, refreshToken) = await tokenService.GenerateTokensAsync(user!);

        return Ok(new { 
            access_token = accessToken,
            refresh_token = refreshToken
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(string token)
    {
        var result = await tokenService.RefreshTokenAsync(token);
        
        if (result == null)
            return Unauthorized(new { message = "Invalid refresh token" });

        var (accessToken, refreshToken) = result.Value;
        
        return Ok(new {
            access_token = accessToken,
            refresh_token = refreshToken
        });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(string token)
    {
        await tokenService.RevokeRefreshTokenAsync(token);
        return Ok(new { message = "Token revoked successfully" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (refreshToken != null)
            await tokenService.RevokeRefreshTokenAsync(refreshToken);
        
        // Clear the refresh token cookie if it exists
        Response.Cookies.Delete("refresh_token");
        
        return Ok(new { message = "Logged out successfully" });
    }
}