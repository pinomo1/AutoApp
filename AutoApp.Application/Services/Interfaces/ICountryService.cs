using AutoApp.Application.DTOs.Queries.CountryQueries;
using AutoApp.Application.DTOs.Responses.CountryResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;

namespace AutoApp.Application.Services.Interfaces;

/// <summary>
/// Contract for country CRUD operations and search
/// </summary>
public interface ICountryService
{
    /// <summary>
    /// Searches countries by optional filters
    /// </summary>
    /// <param name="dto">Search criteria</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching countries with total count</returns>
    Task<CountedResult<CountryResponseDto>> SearchAsync(
        CountrySearchDto dto,
        CancellationToken ct);

    /// <summary>
    /// Gets a country by identifier
    /// </summary>
    /// <param name="id">Country GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Country response DTO</returns>
    Task<CountryResponseDto> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Creates a new country
    /// </summary>
    /// <param name="dto">Create request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created country GUID</returns>
    Task<Guid> CreateAsync(CreateCountryDto dto, CancellationToken ct);

    /// <summary>
    /// Updates an existing country
    /// </summary>
    /// <param name="id">Country GUID</param>
    /// <param name="dto">Update request DTO</param>
    /// <param name="ct">Cancellation token</param>
    Task UpdateAsync(Guid id, UpdateCountryDto dto, CancellationToken ct);

    /// <summary>
    /// Deletes a country by identifier
    /// </summary>
    /// <param name="id">Country GUID</param>
    /// <param name="ct">Cancellation token</param>
    Task DeleteAsync(Guid id, CancellationToken ct);
}