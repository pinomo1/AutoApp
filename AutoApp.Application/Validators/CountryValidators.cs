using AutoApp.Application.DTOs.Queries.CountryQueries;
using FluentValidation;

namespace AutoApp.Application.Validators;

public class CreateCountryDtoValidator : AbstractValidator<CreateCountryDto>
{
    public CreateCountryDtoValidator()
    {
        RuleFor(x => x.CountryName)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .Length(2);
    }
}

public class UpdateCountryDtoValidator : AbstractValidator<UpdateCountryDto>
{
    public UpdateCountryDtoValidator()
    {
        RuleFor(x => x.CountryName)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .Length(2);
    }
}

public class CountrySearchDtoValidator : AbstractValidator<CountrySearchDto>
{
    public CountrySearchDtoValidator()
    {
        RuleFor(x => x.CountryName)
            .MaximumLength(32)
            .When(x => !string.IsNullOrWhiteSpace(x.CountryName));
    }
}