namespace AutoApp.Application.DTOs.Queries.CarPhotoQueries;

/// <summary>
/// Request to update a car photo from an uploaded file.
/// </summary>
public sealed record UpdateCarPhotoUploadDto(
    Guid CarId,
    Guid Id,
    Stream Content,
    string FileName,
    int DisplayOrder = 0,
    bool IsMainPhoto = false);
