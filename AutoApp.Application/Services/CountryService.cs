using AutoApp.Application.DTOs.Queries.CountryQueries;
using AutoApp.Application.DTOs.Responses.CountryResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Exceptions;
using AutoApp.Application.Mappings;
using AutoApp.Application.Services.Interfaces;
using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AutoApp.Application.Services;

/// <summary>
/// Service for country CRUD operations and search
/// </summary>
/// <param name="db">Database context abstraction</param>
public class CountryService(IAutoDbContext db) : ICountryService
{
    private const string CaseInsensitiveCollation = "SQL_Latin1_General_CP1_CI_AS";

    /// <summary>
    /// Searches countries by optional name filter
    /// </summary>
    /// <param name="dto">Search criteria</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching countries with total count</returns>
    public async Task<CountedResult<CountryResponseDto>> SearchAsync(CountrySearchDto dto, CancellationToken ct)
    {
        var items = db.Countries.AsNoTracking().AsQueryable();
        var name = dto.CountryName?.Trim();
        if (!name.IsNullOrEmpty())
        {
            items = items.Where(c =>
                EF.Functions.Like(EF.Functions.Collate(c.CountryName, CaseInsensitiveCollation), $"%{name}%") ||
                EF.Functions.Like(EF.Functions.Collate(c.CountryCode, CaseInsensitiveCollation), $"%{name}%"));
        }

        var totalCount = await items.CountAsync(ct);
        items = items.OrderBy(c => c.CountryName);

        var itemsDto = await items.Select(c => c.ToDto()).ToListAsync(ct);

        return new CountedResult<CountryResponseDto>(itemsDto, totalCount);
    }

    /// <summary>
    /// Gets a country by identifier
    /// </summary>
    /// <param name="id">Country GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Country response DTO</returns>
    /// <exception cref="NotFoundException">Thrown when the country does not exist.</exception>
    public async Task<CountryResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var country = await db.Countries.FirstOrDefaultAsync(c => c.Id == id, ct);
        return country?.ToDto() ?? throw new NotFoundException(nameof(country), id);
    }

    /// <summary>
    /// Creates a new country
    /// </summary>
    /// <param name="dto">Create request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created country GUID</returns>
    public async Task<Guid> CreateAsync(CreateCountryDto dto, CancellationToken ct)
    {
        var country = dto.ToEntity();
        db.Countries.Add(country);
        await db.SaveChangesAsync(ct);
        return country.Id;
    }

    /// <summary>
    /// Updates an existing country
    /// </summary>
    /// <param name="id">Country GUID</param>
    /// <param name="dto">Update request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when the country does not exist.</exception>
    public async Task UpdateAsync(Guid id, UpdateCountryDto dto, CancellationToken ct)
    {
        if(!await db.Countries.AnyAsync(c => c.Id == id, ct))
            throw new NotFoundException(nameof(Country), id);
        var country = dto.ToEntity(id);
        db.Countries.Update(country);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Deletes a country by identifier
    /// </summary>
    /// <param name="id">Country GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when the country does not exist.</exception>
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var country = db.Countries.FirstOrDefault(c => c.Id == id);
        db.Countries.Remove(country ?? throw new NotFoundException(nameof(country), id));
        await db.SaveChangesAsync(ct);
    }
}
