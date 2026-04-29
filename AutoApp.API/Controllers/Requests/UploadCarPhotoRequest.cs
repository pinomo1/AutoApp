using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers.Requests;

/// <summary>
/// Multipart request payload for uploading a car photo.
/// </summary>
public class UploadCarPhotoRequest
{
    /// <summary>
    /// Car photo file.
    /// </summary>
    [FromForm(Name = "file")]
    public IFormFile? File { get; init; }

    /// <summary>
    /// Display order for the photo.
    /// </summary>
    [FromForm(Name = "displayOrder")]
    public int DisplayOrder { get; init; }

    /// <summary>
    /// Whether this is the car's main photo.
    /// </summary>
    [FromForm(Name = "isMainPhoto")]
    public bool IsMainPhoto { get; init; }
}
