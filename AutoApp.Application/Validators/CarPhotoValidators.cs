using AutoApp.Application.DTOs.Queries.CarPhotoQueries;
using FluentValidation;

namespace AutoApp.Application.Validators;

public class CreateCarPhotoDtoValidator : AbstractValidator<CreateCarPhotoDto>
{
    public CreateCarPhotoDtoValidator()
    {
        RuleFor(x => x.CarId)
            .NotEmpty();

        RuleFor(x => x.PhotoUrl)
            .NotEmpty()
            .MaximumLength(512);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}

public class UpdateCarPhotoDtoValidator : AbstractValidator<UpdateCarPhotoDto>
{
    public UpdateCarPhotoDtoValidator()
    {
        RuleFor(x => x.PhotoUrl)
            .NotEmpty()
            .MaximumLength(512);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
