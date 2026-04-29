using AutoApp.Application.DTOs.Responses.CarPhotoResponses;
using AutoApp.API.Controllers.Requests;
using AutoApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers;

/// <summary>
/// Controller for car photos
/// </summary>
/// <param name="carPhotoService">Car photo service</param>
[ApiController]
[Route("api/cars/{carId:guid}/photos")]
public class CarPhotosController(ICarPhotoService carPhotoService) : ControllerBase
{
    /// <summary>
    /// Get all photos for a specific car
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of car photos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<CarPhotoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCarId(Guid carId, CancellationToken ct)
        => Ok(await carPhotoService.GetByCarIdAsync(carId, ct));

    /// <summary>
    /// Get a specific car photo by id
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="id">Car photo GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Car photo or 404</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CarPhotoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid carId, Guid id, CancellationToken ct)
        => Ok(await carPhotoService.GetByIdAsync(id, ct));

    /// <summary>
    /// Create a new car photo
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="request">Multipart form payload with file and metadata</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>New photo ID</returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(Guid carId, [FromForm] UploadCarPhotoRequest request, CancellationToken ct)
    {
        var file = request.File;
        if (file == null || file.Length == 0)
            return BadRequest("Photo file is required.");

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedPhotoExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            return BadRequest($"Unsupported photo extension. Allowed extensions: {string.Join(", ", AllowedPhotoExtensions)}.");

        if (file.Length > MaxPhotoSizeBytes)
            return BadRequest($"Photo file is too large. Maximum allowed size is {MaxPhotoSizeBytes / (1024 * 1024)} MB.");

        await using var stream = file.OpenReadStream();
        var id = await carPhotoService.CreateAsync(carId, stream, file.FileName, request.DisplayOrder, request.IsMainPhoto, ct);
        return CreatedAtAction(nameof(GetById), new { carId, id }, id);
    }

    /// <summary>
    /// Update an existing car photo
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="id">Car photo GUID</param>
    /// <param name="request">Multipart form payload with file and metadata</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>200 with ID</returns>
    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid carId, Guid id, [FromForm] UploadCarPhotoRequest request, CancellationToken ct)
    {
        var file = request.File;
        if (file == null || file.Length == 0)
            return BadRequest("Photo file is required.");

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedPhotoExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            return BadRequest($"Unsupported photo extension. Allowed extensions: {string.Join(", ", AllowedPhotoExtensions)}.");

        if (file.Length > MaxPhotoSizeBytes)
            return BadRequest($"Photo file is too large. Maximum allowed size is {MaxPhotoSizeBytes / (1024 * 1024)} MB.");

        await using var stream = file.OpenReadStream();
        await carPhotoService.UpdateAsync(carId, id, stream, file.FileName, request.DisplayOrder, request.IsMainPhoto, ct);
        return Ok(id);
    }

    /// <summary>
    /// Delete a specific car photo
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="id">Car photo GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Nothing</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid carId, Guid id, CancellationToken ct)
    {
        await carPhotoService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Delete all photos for a specific car
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Nothing</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteByCar(Guid carId, CancellationToken ct)
    {
        await carPhotoService.DeleteByCarIdAsync(carId, ct);
        return NoContent();
    }

    private const int MaxPhotoSizeBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedPhotoExtensions = [".png", ".jpg", ".jpeg", ".webp"];
}
