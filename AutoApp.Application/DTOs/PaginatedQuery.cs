namespace AutoApp.Application.DTOs;

public record PaginatedQuery(int Page = 1, int PageSize = 20);