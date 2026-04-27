using AutoApp.Application.DTOs.Queries.BrandQueries;
using FluentValidation;

namespace AutoApp.Application.Validators;

public class CreateBrandDtoValidator : AbstractValidator<CreateBrandDto>
{
    public CreateBrandDtoValidator()
    {
        RuleFor(x => x.BrandName)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.CountryId)
            .NotEmpty();

        RuleFor(x => x.LogoUrl)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));
    }
}

public class UpdateBrandDtoValidator : AbstractValidator<UpdateBrandDto>
{
    public UpdateBrandDtoValidator()
    {
        RuleFor(x => x.BrandName)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.CountryId)
            .NotEmpty();

        RuleFor(x => x.LogoUrl)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));
    }
}

public class BrandSearchDtoValidator : AbstractValidator<BrandSearchDto>
{
    public BrandSearchDtoValidator()
    {
        RuleFor(x => x.BrandName)
            .MaximumLength(32)
            .When(x => !string.IsNullOrWhiteSpace(x.BrandName));

        RuleFor(x => x.CountryId)
            .NotEmpty()
            .When(x => x.CountryId.HasValue);
    }
}