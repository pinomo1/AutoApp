namespace AutoApp.Application.DTOs.Responses.AuthResponses;

public sealed record MeResponseDto(
    Guid Id,
    string UserName,
    string? Email,
    IReadOnlyCollection<string> Roles);
