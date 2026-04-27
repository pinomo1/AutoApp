using AutoApp.Application.DTOs.Queries.BrandQueries;
using AutoApp.Application.DTOs.Responses.BrandResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Exceptions;
using AutoApp.Application.Mappings;
using AutoApp.Application.Services.Interfaces;
using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Persistence.DbContexts;
using AutoApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AutoApp.Application.Services;

/// <summary>
/// Service for brand CRUD operations and search
/// </summary>
/// <param name="db">Database context abstraction</param>
/// <param name="brandLogoStorage">Brand logo storage abstraction</param>
public class BrandService(IAutoDbContext db, IBrandLogoStorage brandLogoStorage) : IBrandService
{
    private const string CaseInsensitiveCollation = "SQL_Latin1_General_CP1_CI_AS";
    
    /// <summary>
    /// Searches brands by optional name and country filters
    /// </summary>
    /// <param name="dto">Search criteria</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching brands with total count</returns>
    public async Task<CountedResult<BrandResponseDto>> SearchAsync(BrandSearchDto dto, CancellationToken ct)
    {
        var items = db.Brands.AsNoTracking().Include(b => b.Country).AsQueryable();
        var name = dto.BrandName?.Trim();
        if (!name.IsNullOrEmpty())
        {
            items = items.Where(b =>
                EF.Functions.Like(EF.Functions.Collate(b.BrandName, CaseInsensitiveCollation), $"%{name}%"));
        }

        var countryId = dto.CountryId;
        if (countryId != null)
        {
            items = items.Where(b => b.CountryId == countryId);
        }
        var totalCount = await items.CountAsync(ct);
        items = items.OrderBy(b => b.BrandName);

        var itemsDto = await items.Select(b => b.ToDto()).ToListAsync(ct);
        
        return new CountedResult<BrandResponseDto>(itemsDto, totalCount);
    }

    /// <summary>
    /// Gets a brand by identifier
    /// </summary>
    /// <param name="id">Brand GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Brand response DTO</returns>
    /// <exception cref="NotFoundException">Thrown when the brand does not exist.</exception>
    public async Task<BrandResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var brand = await db.Brands.Include(b => b.Country).FirstOrDefaultAsync(b => b.Id == id, ct);
        return brand?.ToDto() ?? throw new NotFoundException(nameof(brand), id);
    }

    /// <summary>
    /// Creates a new brand
    /// </summary>
    /// <param name="dto">Create request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created brand GUID</returns>
    public async Task<Guid> CreateAsync(CreateBrandDto dto, CancellationToken ct)
    {
        var country = db.Countries.FirstOrDefault(c => c.Id == dto.CountryId);
        if (country == null)
            throw new NotFoundException(nameof(Country), dto.CountryId);
        
        var brand = dto.ToEntity();
        country.Brands.Add(brand);
        await db.SaveChangesAsync(ct);
        return brand.Id;
    }

    /// <summary>
    /// Updates an existing brand
    /// </summary>
    /// <param name="id">Brand GUID</param>
    /// <param name="dto">Update request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when the brand or related country does not exist.</exception>
    public async Task UpdateAsync(Guid id, UpdateBrandDto dto, CancellationToken ct)
    {
        if(!await db.Brands.AnyAsync(b => b.Id == id, ct))
            throw new NotFoundException(nameof(Brand), id);
        var brand = dto.ToEntity(id);
        db.Brands.Attach(brand);
        
        var country = db.Countries.FirstOrDefault(c => c.Id == dto.CountryId);
        if (country == null)
            throw new NotFoundException(nameof(Country), dto.CountryId);
        db.Countries.Attach(country);
        
        country.Brands.Add(brand);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Uploads a brand logo and saves the resulting storage path/URL on the brand.
    /// </summary>
    /// <param name="id">Brand GUID</param>
    /// <param name="content">File content stream</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated brand response DTO</returns>
    /// <exception cref="NotFoundException">Thrown when the brand does not exist.</exception>
    public async Task<BrandResponseDto> UploadLogoAsync(Guid id, Stream content, string fileName, CancellationToken ct)
    {
        var brand = await db.Brands.Include(b => b.Country).FirstOrDefaultAsync(b => b.Id == id, ct);
        if (brand == null)
            throw new NotFoundException(nameof(Brand), id);

        var logoUrl = await brandLogoStorage.UploadAsync(id, content, fileName, ct);
        brand.LogoUrl = logoUrl;
        await db.SaveChangesAsync(ct);

        return brand.ToDto();
    }

    /// <summary>
    /// Deletes a brand by identifier
    /// </summary>
    /// <param name="id">Brand GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when the brand does not exist.</exception>
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var brand = db.Brands.FirstOrDefault(b => b.Id == id);
        db.Brands.Remove(brand ?? throw new NotFoundException(nameof(Brand), id));
        await db.SaveChangesAsync(ct);
    }
}