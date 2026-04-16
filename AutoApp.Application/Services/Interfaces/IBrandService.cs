using AutoApp.Application.DTOs.Queries.BrandQueries;
using AutoApp.Application.DTOs.Responses.BrandResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;

namespace AutoApp.Application.Services.Interfaces;

/// <summary>
/// Contract for brand CRUD operations and search
/// </summary>
public interface IBrandService
{
    /// <summary>
    /// Searches brands by optional filters
    /// </summary>
    /// <param name="dto">Search criteria</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching brands with total count</returns>
    Task<CountedResult<BrandResponseDto>> SearchAsync(
        BrandSearchDto dto,
        CancellationToken ct);

    /// <summary>
    /// Gets a brand by identifier
    /// </summary>
    /// <param name="id">Brand GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Brand response DTO</returns>
    Task<BrandResponseDto> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Creates a new brand
    /// </summary>
    /// <param name="dto">Create request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created brand GUID</returns>
    Task<Guid> CreateAsync(CreateBrandDto dto, CancellationToken ct);

    /// <summary>
    /// Updates an existing brand
    /// </summary>
    /// <param name="id">Brand GUID</param>
    /// <param name="dto">Update request DTO</param>
    /// <param name="ct">Cancellation token</param>
    Task UpdateAsync(Guid id, UpdateBrandDto dto, CancellationToken ct);

    /// <summary>
    /// Deletes a brand by identifier
    /// </summary>
    /// <param name="id">Brand GUID</param>
    /// <param name="ct">Cancellation token</param>
    Task DeleteAsync(Guid id, CancellationToken ct);
}