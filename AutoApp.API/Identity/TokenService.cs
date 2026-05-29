using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoApp.Application.DTOs.Responses.AuthResponses;
using AutoApp.Infrastructure.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AutoApp.API.Identity;

/// <summary>
/// Service for managing JWT tokens and refresh tokens.
/// </summary>
/// <param name="userManager"></param>
/// <param name="jwtOptions"></param>
public sealed class TokenService(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtOptions> jwtOptions)
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);

    /// <summary>
    /// Issues a new session with access token and refresh token.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<LoginResponseDto> IssueSessionAsync(ApplicationUser user, CancellationToken ct)
    {
        var roles = (await userManager.GetRolesAsync(user)).ToArray();
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTimeOffset.UtcNow.Add(RefreshTokenLifetime);

        await StoreRefreshTokenAsync(user, refreshToken, refreshTokenExpiresAt);

        return BuildSession(user, roles, refreshToken, refreshTokenExpiresAt);
    }

    /// <summary>
    /// Refreshes the session using a refresh token.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<LoginResponseDto?> RefreshSessionAsync(string refreshToken, CancellationToken ct)
    {
        var tokenHash = HashRefreshToken(refreshToken);
        var now = DateTimeOffset.UtcNow;

        var user = await userManager.Users.FirstOrDefaultAsync(
            candidate => candidate.RefreshTokenHash == tokenHash &&
                         candidate.RefreshTokenExpiresAt.HasValue &&
                         candidate.RefreshTokenExpiresAt.Value > now,
            ct);

        if (user is null)
        {
            return null;
        }

        return await IssueSessionAsync(user, ct);
    }

    /// <summary>
    /// Gets the current user from the claims principal.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<MeResponseDto?> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user is null)
        {
            return null;
        }

        var roles = (await userManager.GetRolesAsync(user)).ToArray();
        return new MeResponseDto(user.Id, user.UserName ?? string.Empty, user.Email, roles);
    }

    /// <summary>
    /// Logs out the user by clearing the refresh token.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<bool> LogoutAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user is null)
        {
            return false;
        }

        user.RefreshTokenHash = null;
        user.RefreshTokenExpiresAt = null;

        var updateResult = await userManager.UpdateAsync(user);
        return updateResult.Succeeded;
    }

    private async Task StoreRefreshTokenAsync(ApplicationUser user, string refreshToken, DateTimeOffset expiresAt)
    {
        user.RefreshTokenHash = HashRefreshToken(refreshToken);
        user.RefreshTokenExpiresAt = expiresAt;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", updateResult.Errors.Select(error => error.Description)));
        }
    }

    private LoginResponseDto BuildSession(
        ApplicationUser user,
        IReadOnlyCollection<string> roles,
        string refreshToken,
        DateTimeOffset refreshTokenExpiresAt)
    {
        var options = jwtOptions.Value;
        var accessTokenExpiresAt = DateTimeOffset.UtcNow.AddMinutes(options.ExpiresMinutes);
        var accessToken = CreateAccessToken(user, roles, accessTokenExpiresAt);

        return new LoginResponseDto(
            accessToken,
            accessTokenExpiresAt,
            refreshToken,
            refreshTokenExpiresAt,
            user.UserName ?? string.Empty,
            roles);
    }

    private string CreateAccessToken(ApplicationUser user, IEnumerable<string> roles, DateTimeOffset expiresAt)
    {
        var options = jwtOptions.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwt = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private static string GenerateRefreshToken()
        => WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

    private static string HashRefreshToken(string refreshToken)
        => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
}
