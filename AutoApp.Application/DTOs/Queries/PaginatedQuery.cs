using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AutoApp.Application.DTOs.Queries;

/// <summary>
/// Query for pagination
/// </summary>
/// <param name="Page">Current page for pagination</param>
/// <param name="PageSize">Page size for pagination</param>
public record PaginatedQuery(
    [DefaultValue(1)]
    [Range(1, int.MaxValue)]
    int Page = 1,
    [DefaultValue(20)]
    [Range(20, 100)]
    int PageSize = 20
);