namespace AutoApp.Application.DTOs.Requests.AuthRequests;

public sealed record LoginRequestDto(string UserNameOrEmail, string Password);
