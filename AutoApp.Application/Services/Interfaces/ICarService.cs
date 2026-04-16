using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Application.DTOs.Responses.CarResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;

namespace AutoApp.Application.Services.Interfaces;

/// <summary>
/// Contract for car CRUD operations and search
/// </summary>
public interface ICarService
{
    /// <summary>
    /// Searches cars with filters, sorting, and pagination
    /// </summary>
    /// <param name="dto">Search request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated car list items</returns>
    Task<PaginatedResult<CarListItemResponseDto>> SearchAsync(
        CarSearchDto dto,
        CancellationToken ct);

    /// <summary>
    /// Gets a car by identifier
    /// </summary>
    /// <param name="id">Car GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Detailed car response DTO</returns>
    Task<CarDetailsResponseDto> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Creates a new car
    /// </summary>
    /// <param name="dto">Create request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created car GUID</returns>
    Task<Guid> CreateAsync(CreateCarDto dto, CancellationToken ct);

    /// <summary>
    /// Updates an existing car
    /// </summary>
    /// <param name="id">Car GUID</param>
    /// <param name="dto">Update request DTO</param>
    /// <param name="ct">Cancellation token</param>
    Task UpdateAsync(Guid id, UpdateCarDto dto, CancellationToken ct);

    /// <summary>
    /// Deletes a car by identifier
    /// </summary>
    /// <param name="id">Car GUID</param>
    /// <param name="ct">Cancellation token</param>
    Task DeleteAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Adds a feature to a car
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="featureId">Feature GUID</param>
    /// <param name="ct">Cancellation token</param>
    Task AddFeatureAsync(Guid carId, Guid featureId, CancellationToken ct);

    /// <summary>
    /// Removes a feature from a car
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="featureId">Feature GUID</param>
    /// <param name="ct">Cancellation token</param>
    Task RemoveFeatureAsync(Guid carId, Guid featureId, CancellationToken ct);
}