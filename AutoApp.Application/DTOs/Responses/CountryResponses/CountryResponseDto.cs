namespace AutoApp.Application.DTOs.Responses.CountryResponses;

/// <summary>
/// Country response payload
/// </summary>
/// <param name="Id">Country GUID</param>
/// <param name="CountryName">Country name</param>
/// <param name="CountryCode">Country code</param>
public record CountryResponseDto(Guid Id, string CountryName, string CountryCode);
