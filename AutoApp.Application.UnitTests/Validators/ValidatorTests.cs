using AutoApp.Application.DTOs.Queries.BrandQueries;
using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Application.DTOs.Queries.SharedQueries;
using AutoApp.Application.Validators;
using AutoApp.Domain.Enums;

namespace AutoApp.Application.UnitTests.Validators;

public class ValidatorTests
{
    [Test]
    public void CreateBrandDtoValidator_WhenBrandNameTooLong_ShouldFail()
    {
        var validator = new CreateBrandDtoValidator();
        var dto = new CreateBrandDto(new string('a', 33), Guid.NewGuid());

        var result = validator.Validate(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(x => x.PropertyName == nameof(CreateBrandDto.BrandName)), Is.True);
        });
    }

    [Test]
    public void PaginatedQueryValidator_WhenPageSizeOutOfRange_ShouldFail()
    {
        var validator = new PaginatedQueryValidator();
        var query = new PaginatedQuery(Page: 1, PageSize: 10);

        var result = validator.Validate(query);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(x => x.PropertyName == nameof(PaginatedQuery.PageSize)), Is.True);
        });
    }

    [Test]
    public void CarFiltersValidator_WhenYearTooLow_ShouldFail()
    {
        var validator = new CarFiltersValidator();
        var filters = new CarFilters(
            SearchString: null,
            BrandName: null,
            BrandId: null,
            CarCondition: CarCondition.Used,
            CarType: null,
            FuelType: null,
            TransmissionType: null,
            Color: null,
            Year: 1700);

        var result = validator.Validate(filters);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(x => x.PropertyName == nameof(CarFilters.Year)), Is.True);
        });
    }

    [Test]
    public void CarSearchDtoValidator_WhenNestedPaginationIsInvalid_ShouldFail()
    {
        var validator = new CarSearchDtoValidator();
        var dto = new CarSearchDto
        {
            Query = new PaginatedQuery(Page: 0, PageSize: 10),
            Filters = new CarFilters(),
            Sorting = new CarSorting()
        };

        var result = validator.Validate(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(x => x.PropertyName.Contains(nameof(CarSearchDto.Query))), Is.True);
        });
    }
}