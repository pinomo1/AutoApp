using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Application.DTOs.Responses.CarResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Exceptions;
using AutoApp.Application.Mappings;
using AutoApp.Application.Services.Interfaces;
using AutoApp.Domain.Enums;
using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.Services;

/// <summary>
/// Service for car CRUD operations and search
/// </summary>
/// <param name="db">Database context abstraction</param>
public class CarService(IAutoDbContext db) : ICarService
{
    private const string CaseInsensitiveCollation = "SQL_Latin1_General_CP1_CI_AS";

    /// <summary>
    /// Searches cars using filters, text terms, sorting, and pagination
    /// </summary>
    /// <param name="dto">Search request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated car list items</returns>
    public async Task<PaginatedResult<CarListItemResponseDto>> SearchAsync(
        CarSearchDto dto,
        CancellationToken ct)
    {
        var items = db.Cars.AsNoTracking().Include(c => c.Brand)
            .ThenInclude(b => b.Country).Include(c => c.CarPhotos).AsQueryable();
        items = ApplyBaseFilters(items, dto.Filters);
        items = ApplySearchWords(items, dto.Filters.SearchString);

        var totalCount = await items.CountAsync(ct);

        items = ApplySorting(items, dto.Sorting);

        var itemsPaginated = await items
            .Skip((dto.Query.Page - 1) * dto.Query.PageSize)
            .Take(dto.Query.PageSize)
            .ToListAsync(ct);

        return new PaginatedResult<CarListItemResponseDto>(
            itemsPaginated.Select(c => c.ToListItemDto(c.CarPhotos.FirstOrDefault(p => p.IsMainPhoto)?.PhotoUrl ?? string.Empty)).ToList(),
            dto.Query.Page,
            dto.Query.PageSize,
            totalCount);
    }

    /// <summary>
    /// Applies exact and partial-match filters except full-text search
    /// </summary>
    /// <param name="items">Source query</param>
    /// <param name="filters">Filter values</param>
    /// <returns>Filtered query</returns>
    private static IQueryable<Car> ApplyBaseFilters(IQueryable<Car> items, CarFilters filters)
    {
        if (filters.BrandId != null)
        {
            items = items.Where(c => c.BrandId == filters.BrandId);
        }

        if (!string.IsNullOrWhiteSpace(filters.BrandName))
        {
            var brand = filters.BrandName.Trim();
            items = items.Where(c =>
                EF.Functions.Like(EF.Functions.Collate(c.Brand.BrandName, CaseInsensitiveCollation), $"%{brand}%"));
        }

        if (filters.CarCondition.HasValue)
            items = items.Where(c => c.CarCondition == filters.CarCondition.Value);

        if (filters.CarType.HasValue)
            items = items.Where(c => c.CarType == filters.CarType.Value);

        if (filters.FuelType.HasValue)
            items = items.Where(c => c.FuelType == filters.FuelType.Value);

        if (filters.TransmissionType.HasValue)
            items = items.Where(c => c.TransmissionType == filters.TransmissionType.Value);

        if (filters.Color.HasValue)
            items = items.Where(c => c.Color == filters.Color.Value);

        if (!filters.Year.HasValue) return items;

        var year = filters.Year.Value;
        items = items.Where(c => c.Year == year);

        return items;
    }

    /// <summary>
    /// Applies word-by-word full-text search across brand, model, color, and year
    /// </summary>
    /// <param name="items">Source query</param>
    /// <param name="searchString">Whitespace-separated search terms</param>
    /// <returns>Filtered query</returns>
    private static IQueryable<Car> ApplySearchWords(IQueryable<Car> items, string? searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return items;
        }

        var searchWords = searchString
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var word in searchWords)
        {
            var pattern = $"%{word}%";
            var hasYear = int.TryParse(word, out var yearTerm);
            var hasCarCondition = Enum.TryParse<CarCondition>(word, true, out var carConditionTerm);
            var hasCarType = Enum.TryParse<CarType>(word, true, out var carTypeTerm);
            var hasFuelType = Enum.TryParse<FuelType>(word, true, out var fuelTypeTerm);
            var hasTransmissionType = Enum.TryParse<TransmissionType>(word, true, out var transmissionTypeTerm);
            var hasColor = Enum.TryParse<Color>(word, true, out var colorTerm);

            items = items.Where(c =>
                EF.Functions.Like(EF.Functions.Collate(c.Brand.BrandName, CaseInsensitiveCollation), pattern) ||
                EF.Functions.Like(EF.Functions.Collate(c.Model, CaseInsensitiveCollation), pattern) ||
                (hasYear && c.Year == yearTerm) ||
                (hasCarCondition && c.CarCondition == carConditionTerm) ||
                (hasCarType && c.CarType == carTypeTerm) ||
                (hasFuelType && c.FuelType == fuelTypeTerm) ||
                (hasTransmissionType && c.TransmissionType == transmissionTypeTerm) ||
                (hasColor && c.Color == colorTerm));
        }

        return items;
    }

    /// <summary>
    /// Applies sorting to the query, defaulting to newest first
    /// </summary>
    /// <param name="items">Source query</param>
    /// <param name="sorting">Sorting settings</param>
    /// <returns>Ordered query</returns>
    private static IQueryable<Car> ApplySorting(IQueryable<Car> items, CarSorting sorting)
    {
        if (sorting.SortType == null)
        {
            return items.OrderByDescending(c => c.Id);
        }

        return sorting.SortType switch
        {
            CarSortType.MileageAscending => items.OrderBy(c => c.Mileage),
            CarSortType.MileageDescending => items.OrderByDescending(c => c.Mileage),
            CarSortType.YearAscending => items.OrderBy(c => c.Year),
            CarSortType.YearDescending => items.OrderByDescending(c => c.Year),
            CarSortType.PriceAscending => items.OrderBy(c => c.Price),
            CarSortType.PriceDescending => items.OrderByDescending(c => c.Price),
            _ => items.OrderByDescending(c => c.Id)
        };
    }

    /// <summary>
    /// Gets a car by identifier
    /// </summary>
    /// <param name="id">Car GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Detailed car response DTO</returns>
    /// <exception cref="NotFoundException">Thrown when the car does not exist.</exception>
    public async Task<CarDetailsResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var car = await db.Cars.Include(c => c.Brand)
            .ThenInclude(b => b.Country).Include(c => c.Features).FirstOrDefaultAsync(c => c.Id == id, ct);
        return car?.ToDto()  ?? throw new NotFoundException(nameof(car), id);
    }

    /// <summary>
    /// Creates a new car
    /// </summary>
    /// <param name="dto">Create request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created car GUID</returns>
    public async Task<Guid> CreateAsync(CreateCarDto dto, CancellationToken ct)
    {
        var brand = db.Brands.FirstOrDefault(b => b.Id == dto.BrandId);
        if (brand == null)
            throw new NotFoundException(nameof(Brand), dto.BrandId);

        var car = dto.ToEntity();
        brand.Cars.Add(car);
        await db.SaveChangesAsync(ct);
        return car.Id;
    }

    /// <summary>
    /// Updates an existing car
    /// </summary>
    /// <param name="id">Car GUID</param>
    /// <param name="dto">Update request DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when the car or related brand does not exist.</exception>
    public async Task UpdateAsync(Guid id, UpdateCarDto dto, CancellationToken ct)
    {
        if(!await db.Cars.AnyAsync(c => c.Id == id, ct))
            throw new NotFoundException(nameof(Car), id);

        var brand = db.Brands.FirstOrDefault(b => b.Id == dto.BrandId);
        if (brand == null)
            throw new NotFoundException(nameof(Brand), dto.BrandId);

        var car = dto.ToEntity(id);
        db.Cars.Update(car);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Adds a feature to an existing car
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="featureId">Feature GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when car or feature does not exist.</exception>
    public async Task AddFeatureAsync(Guid carId, Guid featureId, CancellationToken ct)
    {
        if (!await db.Cars.AnyAsync(c => c.Id == carId, ct))
            throw new NotFoundException(nameof(Car), carId);

        if (!await db.Features.AnyAsync(f => f.Id == featureId, ct))
            throw new NotFoundException(nameof(Feature), featureId);

        var relationExists = await db.CarFeature
            .AnyAsync(cf => cf.CarId == carId && cf.FeatureId == featureId, ct);
        if (relationExists)
            return;

        db.CarFeature.Add(new CarFeature
        {
            CarId = carId,
            FeatureId = featureId
        });
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Removes a feature from an existing car
    /// </summary>
    /// <param name="carId">Car GUID</param>
    /// <param name="featureId">Feature GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when car, feature, or their relation does not exist.</exception>
    public async Task RemoveFeatureAsync(Guid carId, Guid featureId, CancellationToken ct)
    {
        if (!await db.Cars.AnyAsync(c => c.Id == carId, ct))
            throw new NotFoundException(nameof(Car), carId);

        if (!await db.Features.AnyAsync(f => f.Id == featureId, ct))
            throw new NotFoundException(nameof(Feature), featureId);

        var relation = await db.CarFeature
            .FirstOrDefaultAsync(cf => cf.CarId == carId && cf.FeatureId == featureId, ct);

        db.CarFeature.Remove(relation ?? throw new NotFoundException(nameof(CarFeature), featureId));
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Deletes a car by identifier
    /// </summary>
    /// <param name="id">Car GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when the car does not exist.</exception>
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var car = db.Cars.FirstOrDefault(c => c.Id == id);
        db.Cars.Remove(car ?? throw new NotFoundException(nameof(Car), id));
        await db.SaveChangesAsync(ct);
    }
}
