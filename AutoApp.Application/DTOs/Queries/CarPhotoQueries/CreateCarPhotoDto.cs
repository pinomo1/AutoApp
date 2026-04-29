namespace AutoApp.Application.DTOs.Queries.CarPhotoQueries;

/// <summary>
/// Request to create a car photo
/// </summary>
/// <param name="CarId">Car GUID</param>
/// <param name="PhotoUrl">Photo URL or storage path</param>
/// <param name="DisplayOrder">Display order index</param>
/// <param name="IsMainPhoto">Whether this is the main photo</param>
public record CreateCarPhotoDto(
    Guid CarId,
    string PhotoUrl,
    int DisplayOrder = 0,
    bool IsMainPhoto = false);
