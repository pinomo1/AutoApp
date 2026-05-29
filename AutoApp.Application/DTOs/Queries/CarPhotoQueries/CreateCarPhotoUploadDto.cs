namespace AutoApp.Application.DTOs.Queries.CarPhotoQueries;

/// <summary>
/// Request to create a car photo from an uploaded file.
/// </summary>
public sealed record CreateCarPhotoUploadDto(
    Guid CarId,
    Stream Content,
    string FileName,
    int DisplayOrder = 0,
    bool IsMainPhoto = false);
