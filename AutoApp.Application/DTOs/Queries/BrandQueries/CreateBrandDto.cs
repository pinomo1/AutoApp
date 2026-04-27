namespace AutoApp.Application.DTOs.Queries.BrandQueries;

/// <summary>
/// Request to create a brand
/// </summary>
/// <param name="BrandName">Brand name</param>
/// <param name="CountryId">Country GUID</param>
/// <param name="LogoUrl">Optional brand logo/photo URL or storage path</param>
public record CreateBrandDto(string BrandName, Guid CountryId, string? LogoUrl = null);