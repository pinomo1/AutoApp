using AutoApp.API.Controllers;
using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Application.DTOs.Responses.CarResponses;
using AutoApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.UnitTests.Controllers;

public class MetaControllerTests
{
    [Test]
    public void GetCarEnums_WhenCalled_ShouldReturnOkWithEnumOptions()
    {
        var controller = new MetaController();

        var actionResult = controller.GetCarEnums();

        var ok = actionResult as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.TypeOf<CarEnumOptionsResponseDto>());

        var dto = (CarEnumOptionsResponseDto)ok.Value!;
        Assert.Multiple(() =>
        {
            Assert.That(dto.CarConditions, Is.EquivalentTo(Enum.GetNames<CarCondition>()));
            Assert.That(dto.CarTypes, Is.EquivalentTo(Enum.GetNames<CarType>()));
            Assert.That(dto.FuelTypes, Is.EquivalentTo(Enum.GetNames<FuelType>()));
            Assert.That(dto.TransmissionTypes, Is.EquivalentTo(Enum.GetNames<TransmissionType>()));
            Assert.That(dto.Colors, Is.EquivalentTo(Enum.GetNames<Color>()));
            Assert.That(dto.CarSortTypes, Is.EquivalentTo(Enum.GetNames<CarSortType>()));
        });
    }
}