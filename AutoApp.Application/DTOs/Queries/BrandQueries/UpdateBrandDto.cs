namespace AutoApp.Application.DTOs.Queries.BrandQueries;

/// <summary>
/// Request to update an existing brand
/// </summary>
/// <param name="BrandName">New brand name</param>
/// <param name="CountryId">New country GUID</param>
public record UpdateBrandDto(string BrandName, Guid CountryId);
