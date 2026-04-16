namespace AutoApp.Application.DTOs.Responses.BrandResponses;

/// <summary>
/// Brand response payload
/// </summary>
/// <param name="Id">Brand GUID</param>
/// <param name="BrandName">Brand name</param>
/// <param name="CountryId">Country GUID</param>
/// <param name="CountryName">Country name</param>
public record BrandResponseDto(Guid Id, string BrandName, Guid CountryId, string CountryName);