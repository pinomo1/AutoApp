using AutoApp.Application.DTOs;
using AutoApp.Application.Exceptions;
using AutoApp.Application.Mappings;
using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.Services;

public class CarService(IAutoDbContext db) : ICarService
{
    public async Task<PaginatedResult<ResponseCarDto>> GetAllAsync(PaginatedQuery query, CarFilters filters, CancellationToken ct)
    {
        var items = db.Cars.Where(c =>
            c.Brand.Contains(filters.Brand ?? "") &&
            c.Color.Contains(filters.Color ?? "") &&
            c.Year == (filters.Year ?? c.Year));
        
        var totalCount = await items.CountAsync(ct);

        var itemsPaginated = await items
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new ResponseCarDto(c.Id, c.Brand, c.Model, c.Year, c.Color, c.Price, c.Mileage))
            .ToListAsync(ct);

        return new PaginatedResult<ResponseCarDto>(itemsPaginated, query.Page, query.PageSize, totalCount);
    }

    public async Task<ResponseCarDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var car = await db.Cars.FindAsync([id], ct);
        if (car == null) throw new NotFoundException(nameof(Car), id);
        return car.ToDto();
    }

    public async Task<Guid> CreateAsync(CreateCarDto dto, CancellationToken ct)
    {
        var car = new Car()
        {
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            Color = dto.Color,
            Price = dto.Price,
            Mileage = dto.Mileage
        };
        db.Cars.Add(car);
        await db.SaveChangesAsync(ct);
        return car.Id;
    }

    public async Task UpdateAsync(UpdateCarDto dto, CancellationToken ct)
    {
        var car = new Car()
        {
            Id = dto.Id,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            Color = dto.Color,
            Price = dto.Price,
            Mileage = dto.Mileage
        };
        db.Cars.Update(car);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var car = await GetByIdAsync(id, ct);
        db.Cars.Remove(car.ToEntity());
        await db.SaveChangesAsync(ct);
    }
}