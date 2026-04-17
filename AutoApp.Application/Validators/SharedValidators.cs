using AutoApp.Application.DTOs.Queries.SharedQueries;
using FluentValidation;

namespace AutoApp.Application.Validators;

public class PaginatedQueryValidator : AbstractValidator<PaginatedQuery>
{
    public PaginatedQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(20, 100);
    }
}