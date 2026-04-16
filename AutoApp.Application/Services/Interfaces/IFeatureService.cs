using AutoApp.Application.DTOs.Queries.FeatureQueries;
using AutoApp.Application.DTOs.Responses.FeatureResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;

namespace AutoApp.Application.Services.Interfaces;

/// <summary>
/// Contract for feature CRUD operations and search
/// </summary>
public interface IFeatureService
{
    /// <summary>
    /// Searches features by optional filters
    /// </summary>
    /// <param name="dto">Search criteria</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching features with total count</returns>
    Task<CountedResult<FeatureResponseDto>> SearchAsync(
        FeatureSearchDto dto,
        CancellationToken ct);

    /// <summary>
    /// Gets a feature by identifier
    /// </summary>
    /// <param name="id">Feature GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Feature response DTO</returns>
    Task<FeatureResponseDto> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Creates a new feature
    /// </summary>
    /// <param name="dto">Create request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created feature GUID</returns>
    Task<Guid> CreateAsync(CreateFeatureDto dto, CancellationToken ct);

    /// <summary>
    /// Updates an existing feature
    /// </summary>
    /// <param name="id">Feature GUID</param>
    /// <param name="dto">Update request DTO</param>
    /// <param name="ct">Cancellation token</param>
    Task UpdateAsync(Guid id, UpdateFeatureDto dto, CancellationToken ct);

    /// <summary>
    /// Deletes a feature by identifier
    /// </summary>
    /// <param name="id">Feature GUID</param>
    /// <param name="ct">Cancellation token</param>
    Task DeleteAsync(Guid id, CancellationToken ct);
}