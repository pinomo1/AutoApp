using AutoApp.Application.DTOs.Queries.FeatureQueries;
using FluentValidation;

namespace AutoApp.Application.Validators;

public class CreateFeatureDtoValidator : AbstractValidator<CreateFeatureDto>
{
    public CreateFeatureDtoValidator()
    {
        RuleFor(x => x.FeatureName)
            .NotEmpty()
            .MaximumLength(32);
    }
}

public class UpdateFeatureDtoValidator : AbstractValidator<UpdateFeatureDto>
{
    public UpdateFeatureDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.FeatureName)
            .NotEmpty()
            .MaximumLength(32);
    }
}

public class FeatureSearchDtoValidator : AbstractValidator<FeatureSearchDto>
{
    public FeatureSearchDtoValidator()
    {
        RuleFor(x => x.FeatureName)
            .MaximumLength(32)
            .When(x => !string.IsNullOrWhiteSpace(x.FeatureName));
    }
}