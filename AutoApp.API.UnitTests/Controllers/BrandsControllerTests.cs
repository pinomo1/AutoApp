using AutoApp.API.Controllers;
using AutoApp.Application.DTOs.Queries.BrandQueries;
using AutoApp.Application.DTOs.Responses.BrandResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoApp.API.UnitTests.Controllers;

public class BrandsControllerTests
{
    [Test]
    public async Task Search_WhenCalled_ShouldReturnOkWithResult()
    {
        var service = new Mock<IBrandService>();
        var dto = new BrandSearchDto("Toy", null);
        var result = new CountedResult<BrandResponseDto>(
            new List<BrandResponseDto> { new(Guid.NewGuid(), "Toyota", Guid.NewGuid(), "Japan") },
            1);

        service.Setup(s => s.SearchAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(result);
        var controller = new BrandsController(service.Object);

        var actionResult = await controller.Search(dto, CancellationToken.None);

        var ok = actionResult as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(result));
    }

    [Test]
    public async Task Update_WhenCalled_ShouldReturnOkWithId()
    {
        var service = new Mock<IBrandService>();
        var id = Guid.NewGuid();
        var dto = new UpdateBrandDto(Guid.NewGuid(), "Toyota", Guid.NewGuid());
        var controller = new BrandsController(service.Object);

        var actionResult = await controller.Update(id, dto, CancellationToken.None);

        var ok = actionResult as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(id));
        service.Verify(s => s.UpdateAsync(id, dto, It.IsAny<CancellationToken>()), Times.Once);
    }
}