using AutoApp.Application.DTOs;
using AutoApp.Domain.Entities;

namespace AutoApp.Application.Services;

public interface ICarService
{
    Task<PaginatedResult<ResponseCarDto>> GetAllAsync(PaginatedQuery query, CarFilters filters, CancellationToken ct);
    Task<ResponseCarDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Guid> CreateAsync(CreateCarDto dto, CancellationToken ct);
    Task UpdateAsync(UpdateCarDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}