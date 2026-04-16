using System.ComponentModel.DataAnnotations;

namespace AutoApp.Application.DTOs.Queries.BrandQueries;

/// <summary>
/// Request to create a brand
/// </summary>
/// <param name="BrandName">Brand name</param>
/// <param name="CountryId">Country GUID</param>
public record CreateBrandDto([Required] [StringLength(32)] string BrandName, [Required] Guid CountryId);