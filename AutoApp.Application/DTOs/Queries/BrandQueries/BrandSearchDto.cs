namespace AutoApp.Application.DTOs.Queries.BrandQueries;

/// <summary>
/// Search criteria for brands
/// </summary>
/// <param name="BrandName">Brand name filter</param>
/// <param name="CountryId">Country GUID filter</param>
public record BrandSearchDto(string? BrandName, Guid? CountryId);