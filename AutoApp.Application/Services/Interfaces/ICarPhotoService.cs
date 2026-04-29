using AutoApp.Application.DTOs.Responses.CarPhotoResponses;

namespace AutoApp.Application.Services.Interfaces;

/// <summary>
/// Contract for car photo CRUD operations and search
/// </summary>
public interface ICarPhotoService
{
    /// <summary>
    /// Gets all photos for a specific car
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of car photo response DTOs</returns>
    Task<List<CarPhotoResponseDto>> GetByCarIdAsync(Guid carId, CancellationToken ct);

    /// <summary>
    /// Gets a car photo by identifier
    /// </summary>
    /// <param name="id">Car photo GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Car photo response DTO</returns>
    Task<CarPhotoResponseDto> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Creates a new car photo
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="content">Uploaded file stream</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="displayOrder">Display order index</param>
    /// <param name="isMainPhoto">Whether this is the main photo</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created car photo GUID</returns>
    Task<Guid> CreateAsync(Guid carId, Stream content, string fileName, int displayOrder, bool isMainPhoto, CancellationToken ct);

    /// <summary>
    /// Updates an existing car photo
    /// </summary>
    /// <param name="id">Car photo GUID</param>
    /// <param name="carId">Car GUID</param>
    /// <param name="content">Uploaded file stream</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="displayOrder">Display order index</param>
    /// <param name="isMainPhoto">Whether this is the main photo</param>
    /// <param name="ct">Cancellation token</param>
    Task UpdateAsync(Guid carId, Guid id, Stream content, string fileName, int displayOrder, bool isMainPhoto, CancellationToken ct);

    /// <summary>
    /// Deletes a car photo by identifier
    /// </summary>
    /// <param name="id">Car photo GUID</param>
    /// <param name="ct">Cancellation token</param>
    Task DeleteAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Deletes all photos for a specific car
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="ct">Cancellation token</param>
    Task DeleteByCarIdAsync(Guid carId, CancellationToken ct);
}
