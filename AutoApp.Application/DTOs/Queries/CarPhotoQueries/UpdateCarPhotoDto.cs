namespace AutoApp.Application.DTOs.Queries.CarPhotoQueries;

/// <summary>
/// Request to update a car photo
/// </summary>
/// <param name="PhotoUrl">Photo URL or storage path</param>
/// <param name="DisplayOrder">Display order index</param>
/// <param name="IsMainPhoto">Whether this is the main photo</param>
public record UpdateCarPhotoDto(
    string PhotoUrl,
    int DisplayOrder = 0,
    bool IsMainPhoto = false);
