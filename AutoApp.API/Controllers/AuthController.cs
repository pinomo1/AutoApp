using AutoApp.API.Identity;
using AutoApp.Application.DTOs.Requests.AuthRequests;
using AutoApp.Application.DTOs.Responses.AuthResponses;
using AutoApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers;

/// <summary>
/// Authentication endpoints.
/// </summary>
/// <param name="userManager">Identity user manager.</param>
/// <param name="tokenService">Token service.</param>
[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    TokenService tokenService) : ControllerBase
{
    /// <summary>
    /// Logs a user in and returns a JWT access token.
    /// </summary>
    /// <param name="dto">Username/email and password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>JWT token response.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequestDto dto, CancellationToken ct)
    {
        var user = await userManager.FindByNameAsync(dto.UserNameOrEmail)
            ?? await userManager.FindByEmailAsync(dto.UserNameOrEmail);

        if (user is null || !await userManager.CheckPasswordAsync(user, dto.Password))
        {
            return Unauthorized();
        }

        return Ok(await tokenService.IssueSessionAsync(user, ct));
    }

    /// <summary>
    /// Refreshes the session using a refresh token.
    /// </summary>
    /// <param name="dto">Refresh token payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>New token pair or 401.</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto, CancellationToken ct)
    {
        var session = await tokenService.RefreshSessionAsync(dto.RefreshToken, ct);
        return session is null ? Unauthorized() : Ok(session);
    }

    /// <summary>
    /// Returns the currently authenticated user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Current user profile.</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(MeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var me = await tokenService.GetCurrentUserAsync(User, ct);
        return me is null ? Unauthorized() : Ok(me);
    }

    /// <summary>
    /// Logs out the currently authenticated user by invalidating their refresh token.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on successful logout.</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var success = await tokenService.LogoutAsync(User, ct);
        return success ? NoContent() : Unauthorized();
    }
}
