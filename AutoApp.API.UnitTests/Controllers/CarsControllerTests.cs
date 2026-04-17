using AutoApp.API.Controllers;
using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Application.DTOs.Responses.CarResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Services.Interfaces;
using AutoApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoApp.API.UnitTests.Controllers;

public class CarsControllerTests
{
    [Test]
    public async Task Search_WhenCalled_ShouldReturnOkWithResult()
    {
        var service = new Mock<ICarService>();
        var dto = new CarSearchDto();
        var result = new PaginatedResult<CarListItemResponseDto>(
            new List<CarListItemResponseDto>
            {
                new(Guid.NewGuid(), "Toyota", "Camry", 2020, "Black", 180, 2500, 20000m, 50000)
            },
            1,
            20,
            1);

        service.Setup(s => s.SearchAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(result);
        var controller = new CarsController(service.Object);

        var actionResult = await controller.Search(dto, CancellationToken.None);

        var ok = actionResult as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(result));
    }

    [Test]
    public async Task AddFeature_WhenCalled_ShouldReturnNoContentAndCallService()
    {
        var service = new Mock<ICarService>();
        var carId = Guid.NewGuid();
        var featureId = Guid.NewGuid();
        var controller = new CarsController(service.Object);

        var actionResult = await controller.AddFeature(carId, featureId, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<NoContentResult>());
        service.Verify(s => s.AddFeatureAsync(carId, featureId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Create_WhenCalled_ShouldReturnCreatedAtActionWithId()
    {
        var service = new Mock<ICarService>();
        var dto = CreateCarDto(Guid.NewGuid());
        var id = Guid.NewGuid();

        service.Setup(s => s.CreateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(id);
        var controller = new CarsController(service.Object);

        var actionResult = await controller.Create(dto, CancellationToken.None);

        var created = actionResult as CreatedAtActionResult;
        Assert.That(created, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(created!.ActionName, Is.EqualTo(nameof(CarsController.GetById)));
            Assert.That(created.Value, Is.EqualTo(id));
        });
    }

    private static CreateCarDto CreateCarDto(Guid brandId)
    {
        return new CreateCarDto(
            brandId,
            "Camry",
            2020,
            CarCondition.Used,
            CarType.Sedan,
            FuelType.Gasoline,
            TransmissionType.Automatic,
            Color.Black,
            180,
            2500,
            20000m,
            50000);
    }
}