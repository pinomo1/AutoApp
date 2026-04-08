using AutoApp.Application.DTOs.Queries;
using AutoApp.Application.DTOs.Responses;

namespace AutoApp.Application.Services;

public interface ICarService
{
    Task<PaginatedResult<ResponseCarDto>> GetAllAsync(
        PaginatedQuery query, 
        CarFilters filters,
        CarSorting sorting,
        CancellationToken ct);
    Task<ResponseCarDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Guid> CreateAsync(CreateCarDto dto, CancellationToken ct);
    Task UpdateAsync(Guid id, UpdateCarDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}