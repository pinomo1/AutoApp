namespace AutoApp.Application.DTOs.Responses.FeatureResponses;

/// <summary>
/// Feature response payload
/// </summary>
/// <param name="Id">Feature GUID</param>
/// <param name="FeatureName">Feature name</param>
public record FeatureResponseDto(Guid Id, string FeatureName);