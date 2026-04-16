using System.ComponentModel.DataAnnotations;

namespace AutoApp.Application.DTOs.Queries.BrandQueries;

/// <summary>
/// Search criteria for brands
/// </summary>
/// <param name="BrandName">Brand name filter</param>
/// <param name="CountryId">Country GUID filter</param>
public record BrandSearchDto([StringLength(32)] string? BrandName,  Guid? CountryId);