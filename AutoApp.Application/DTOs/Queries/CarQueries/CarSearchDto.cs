using AutoApp.Application.DTOs.Queries.SharedQueries;

namespace AutoApp.Application.DTOs.Queries.CarQueries;

/// <summary>
/// Search request for cars
/// </summary>
public record CarSearchDto
{
    /// <summary>
    /// Pagination parameters
    /// </summary>
    public PaginatedQuery Query { get; init; } = new();
    /// <summary>
    /// Search filters
    /// </summary>
    public CarFilters Filters { get; init; } = new();
    /// <summary>
    /// Sorting settings
    /// </summary>
    public CarSorting Sorting { get; init; } = new();
}