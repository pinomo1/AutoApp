namespace AutoApp.Application.DTOs.Queries.BrandQueries;

/// <summary>
/// Request to upload a brand logo.
/// </summary>
public sealed record UploadBrandLogoDto(Guid BrandId, Stream Content, string FileName);
