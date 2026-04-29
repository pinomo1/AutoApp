namespace AutoApp.Application.DTOs.Responses.CarPhotoResponses;

/// <summary>
/// Car photo response payload
/// </summary>
/// <param name="Id">GUID of the car photo</param>
/// <param name="CarId">GUID of the car</param>
/// <param name="PhotoUrl">Photo URL or storage path</param>
/// <param name="DisplayOrder">Display order index</param>
/// <param name="IsMainPhoto">Whether this is the main photo</param>
public record CarPhotoResponseDto(
    Guid Id,
    Guid CarId,
    string PhotoUrl,
    int DisplayOrder,
    bool IsMainPhoto);
