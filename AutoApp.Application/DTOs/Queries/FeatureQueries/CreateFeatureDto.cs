namespace AutoApp.Application.DTOs.Queries.FeatureQueries;

/// <summary>
/// Request to create a feature
/// </summary>
/// <param name="FeatureName">Feature name</param>
public record CreateFeatureDto(string FeatureName);