namespace AutoApp.Application.DTOs.Responses.AuthResponses;

public sealed record LoginResponseDto(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    string UserName,
    IReadOnlyCollection<string> Roles);
