using AutoApp.API.Controllers;
using AutoApp.Application.DTOs.Queries.CountryQueries;
using AutoApp.Application.DTOs.Responses.CountryResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoApp.API.UnitTests.Controllers;

public class CountriesControllerTests
{
    [Test]
    public async Task Search_WhenCalled_ShouldReturnOkWithResult()
    {
        var service = new Mock<ICountryService>();
        var dto = new CountrySearchDto("Ja");
        var result = new CountedResult<CountryResponseDto>(
            new List<CountryResponseDto> { new(Guid.NewGuid(), "Japan", "JP") },
            1);

        service.Setup(s => s.SearchAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(result);
        var controller = new CountriesController(service.Object);

        var actionResult = await controller.Search(dto, CancellationToken.None);

        var ok = actionResult as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(result));
    }

    [Test]
    public async Task Create_WhenCalled_ShouldReturnCreatedAtActionWithId()
    {
        var service = new Mock<ICountryService>();
        var dto = new CreateCountryDto("Japan", "JP");
        var id = Guid.NewGuid();

        service.Setup(s => s.CreateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(id);
        var controller = new CountriesController(service.Object);

        var actionResult = await controller.Create(dto, CancellationToken.None);

        var created = actionResult as CreatedAtActionResult;
        Assert.That(created, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(created!.ActionName, Is.EqualTo(nameof(CountriesController.GetById)));
            Assert.That(created.Value, Is.EqualTo(id));
        });
        service.Verify(s => s.CreateAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Delete_WhenCalled_ShouldReturnNoContent()
    {
        var service = new Mock<ICountryService>();
        var id = Guid.NewGuid();
        var controller = new CountriesController(service.Object);

        var actionResult = await controller.Delete(id, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<NoContentResult>());
        service.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}