namespace AutoApp.Application.DTOs.Responses;

/// <summary>
/// Result of pagination query
/// </summary>
/// <param name="Items">Items returned from the query</param>
/// <param name="Page">Current page for pagination</param>
/// <param name="PageSize">Page size for pagination</param>
/// <param name="TotalCount">Total count of all the items</param>
/// <typeparam name="T">Any class to be used with pagination</typeparam>
public record PaginatedResult<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    /// <summary>
    /// Total pages in pagination
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    /// <summary>
    /// Shows if there is a next page in pagination
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
    /// <summary>
    /// Show if there is a previous page in pagination
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}