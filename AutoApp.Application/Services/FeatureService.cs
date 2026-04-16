using AutoApp.Application.DTOs.Queries.FeatureQueries;
using AutoApp.Application.DTOs.Responses.FeatureResponses;
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
/// Service for feature CRUD operations and search
/// </summary>
/// <param name="db">Database context abstraction</param>
public class FeatureService(IAutoDbContext db) : IFeatureService
{
    private const string CaseInsensitiveCollation = "SQL_Latin1_General_CP1_CI_AS";
    
    /// <summary>
    /// Searches features by optional name filter
    /// </summary>
    /// <param name="dto">Search criteria</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching features with total count</returns>
    public async Task<CountedResult<FeatureResponseDto>> SearchAsync(FeatureSearchDto dto, CancellationToken ct)
    {
        var items = db.Features.AsNoTracking().AsQueryable();
        var name = dto.FeatureName?.Trim();
        if (!name.IsNullOrEmpty())
        {
            items = items.Where(c =>
                EF.Functions.Like(EF.Functions.Collate(c.FeatureName, CaseInsensitiveCollation), $"%{name}%"));
        }
        
        var totalCount = await items.CountAsync(ct);
        items = items.OrderBy(c => c.FeatureName);

        var itemsDto = await items.Select(c => c.ToDto()).ToListAsync(ct);
        
        return new CountedResult<FeatureResponseDto>(itemsDto, totalCount);
    }

    /// <summary>
    /// Gets a feature by identifier
    /// </summary>
    /// <param name="id">Feature GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Feature response DTO</returns>
    /// <exception cref="NotFoundException">Thrown when the feature does not exist.</exception>
    public async Task<FeatureResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var feature = await db.Features.FirstOrDefaultAsync(c => c.Id == id, ct);
        return feature?.ToDto() ?? throw new NotFoundException(nameof(feature), id);
    }

    /// <summary>
    /// Creates a new feature
    /// </summary>
    /// <param name="dto">Create request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created feature GUID</returns>
    public async Task<Guid> CreateAsync(CreateFeatureDto dto, CancellationToken ct)
    {
        var feature = dto.ToEntity();
        db.Features.Add(feature);
        await db.SaveChangesAsync(ct);
        return feature.Id;
    }

    /// <summary>
    /// Updates an existing feature
    /// </summary>
    /// <param name="id">Feature GUID</param>
    /// <param name="dto">Update request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when the feature does not exist.</exception>
    public async Task UpdateAsync(Guid id, UpdateFeatureDto dto, CancellationToken ct)
    {
        if(!await db.Features.AnyAsync(c => c.Id == id, ct))
            throw new NotFoundException(nameof(Feature), id);
        var feature = dto.ToEntity(id);
        db.Features.Update(feature);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Deletes a feature by identifier
    /// </summary>
    /// <param name="id">Feature GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when the feature does not exist.</exception>
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var feature = db.Features.FirstOrDefault(c => c.Id == id);
        db.Features.Remove(feature ?? throw new NotFoundException(nameof(feature), id));
        await db.SaveChangesAsync(ct);
    }
}