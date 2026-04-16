using System.ComponentModel.DataAnnotations;

namespace AutoApp.Application.DTOs.Queries.FeatureQueries;

/// <summary>
/// Request to update an existing brand
/// </summary>
/// <param name="Id">Feature GUID</param>
/// <param name="FeatureName">New feature name</param>
public record UpdateFeatureDto([Required] Guid Id, [Required] [StringLength(32)] string FeatureName);