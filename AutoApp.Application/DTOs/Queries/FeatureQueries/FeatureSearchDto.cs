namespace AutoApp.Application.DTOs.Queries.FeatureQueries;

/// <summary>
/// Search criteria for features
/// </summary>
/// <param name="FeatureName">Feature name filter</param>
public record FeatureSearchDto(string? FeatureName);