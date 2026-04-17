using System.ComponentModel;

namespace AutoApp.Application.DTOs.Queries.SharedQueries;

/// <summary>
/// Pagination parameters for search queries
/// </summary>
/// <param name="Page">Current page number</param>
/// <param name="PageSize">Number of items per page</param>
public record PaginatedQuery(
    [DefaultValue(1)]
    int Page = 1,
    [DefaultValue(20)]
    int PageSize = 20
);