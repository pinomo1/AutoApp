using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AutoApp.Application.DTOs.Queries.SharedQueries;

/// <summary>
/// Pagination parameters for search queries
/// </summary>
/// <param name="Page">Current page number</param>
/// <param name="PageSize">Number of items per page</param>
public record PaginatedQuery(
    [DefaultValue(1)]
    [Range(1, int.MaxValue)]
    int Page = 1,
    [DefaultValue(20)]
    [Range(20, 100)]
    int PageSize = 20
);