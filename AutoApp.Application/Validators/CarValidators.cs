using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Domain.Enums;
using FluentValidation;

namespace AutoApp.Application.Validators;

public class CreateCarDtoValidator : AbstractValidator<CreateCarDto>
{
    public CreateCarDtoValidator()
    {
        RuleFor(x => x.BrandId)
            .NotEmpty();

        RuleFor(x => x.Model)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.Year)
            .InclusiveBetween((short)1800, (short)2026);

        RuleFor(x => x.Horsepower)
            .GreaterThan(0);

        RuleFor(x => x.EngineVolumeCc)
            .GreaterThan(0);

        RuleFor(x => x.Price)
            .InclusiveBetween(0m, 100000000m);

        RuleFor(x => x.Mileage)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.CarCondition)
            .IsInEnum()
            .NotEqual(CarCondition.Undefined);

        RuleFor(x => x.CarType)
            .IsInEnum()
            .NotEqual(CarType.Undefined);

        RuleFor(x => x.FuelType)
            .IsInEnum()
            .NotEqual(FuelType.Undefined);

        RuleFor(x => x.TransmissionType)
            .IsInEnum()
            .NotEqual(TransmissionType.Undefined);

        RuleFor(x => x.Color)
            .IsInEnum()
            .NotEqual(Color.Undefined);
    }
}

public class UpdateCarDtoValidator : AbstractValidator<UpdateCarDto>
{
    public UpdateCarDtoValidator()
    {
        Include(new CreateCarDtoValidatorAdapter());
    }

    private sealed class CreateCarDtoValidatorAdapter : AbstractValidator<UpdateCarDto>
    {
        public CreateCarDtoValidatorAdapter()
        {
            RuleFor(x => x.BrandId)
                .NotEmpty();

            RuleFor(x => x.Model)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(x => x.Year)
                .InclusiveBetween((short)1800, (short)2026);

            RuleFor(x => x.Horsepower)
                .GreaterThan(0);

            RuleFor(x => x.EngineVolumeCc)
                .GreaterThan(0);

            RuleFor(x => x.Price)
                .InclusiveBetween(0m, 100000000m);

            RuleFor(x => x.Mileage)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CarCondition)
                .IsInEnum()
                .NotEqual(CarCondition.Undefined);

            RuleFor(x => x.CarType)
                .IsInEnum()
                .NotEqual(CarType.Undefined);

            RuleFor(x => x.FuelType)
                .IsInEnum()
                .NotEqual(FuelType.Undefined);

            RuleFor(x => x.TransmissionType)
                .IsInEnum()
                .NotEqual(TransmissionType.Undefined);

            RuleFor(x => x.Color)
                .IsInEnum()
                .NotEqual(Color.Undefined);
        }
    }
}

public class CarFiltersValidator : AbstractValidator<CarFilters>
{
    public CarFiltersValidator()
    {
        RuleFor(x => x.SearchString)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.SearchString));

        RuleFor(x => x.BrandName)
            .MaximumLength(32)
            .When(x => !string.IsNullOrWhiteSpace(x.BrandName));

        RuleFor(x => x.BrandId)
            .NotEmpty()
            .When(x => x.BrandId.HasValue);

        RuleFor(x => x.Year)
            .InclusiveBetween((short)1800, (short)2026)
            .When(x => x.Year.HasValue);

        RuleFor(x => x.CarCondition)
            .IsInEnum()
            .NotEqual(CarCondition.Undefined)
            .When(x => x.CarCondition.HasValue);

        RuleFor(x => x.CarType)
            .IsInEnum()
            .NotEqual(CarType.Undefined)
            .When(x => x.CarType.HasValue);

        RuleFor(x => x.FuelType)
            .IsInEnum()
            .NotEqual(FuelType.Undefined)
            .When(x => x.FuelType.HasValue);

        RuleFor(x => x.TransmissionType)
            .IsInEnum()
            .NotEqual(TransmissionType.Undefined)
            .When(x => x.TransmissionType.HasValue);

        RuleFor(x => x.Color)
            .IsInEnum()
            .NotEqual(Color.Undefined)
            .When(x => x.Color.HasValue);
    }
}

public class CarSortingValidator : AbstractValidator<CarSorting>
{
    public CarSortingValidator()
    {
        RuleFor(x => x.SortType)
            .IsInEnum()
            .NotEqual(CarSortType.Undefined)
            .When(x => x.SortType.HasValue);
    }
}

public class CarSearchDtoValidator : AbstractValidator<CarSearchDto>
{
    public CarSearchDtoValidator()
    {
        RuleFor(x => x.Query)
            .SetValidator(new PaginatedQueryValidator());

        RuleFor(x => x.Filters)
            .SetValidator(new CarFiltersValidator());

        RuleFor(x => x.Sorting)
            .SetValidator(new CarSortingValidator());
    }
}