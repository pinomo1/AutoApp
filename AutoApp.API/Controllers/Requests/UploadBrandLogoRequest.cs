using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers.Requests;

/// <summary>
/// Multipart request payload for uploading a brand logo.
/// </summary>
public class UploadBrandLogoRequest
{
    /// <summary>
    /// Brand logo file.
    /// </summary>
    [FromForm(Name = "file")]
    public IFormFile? File { get; init; }
}
