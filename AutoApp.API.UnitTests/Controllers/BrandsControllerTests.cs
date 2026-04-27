using AutoApp.API.Controllers;
using AutoApp.API.Controllers.Requests;
using AutoApp.Application.DTOs.Queries.BrandQueries;
using AutoApp.Application.DTOs.Responses.BrandResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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
            new List<BrandResponseDto> { new(Guid.NewGuid(), "Toyota", Guid.NewGuid(), "Japan", "https://cdn.example.com/toyota.png") },
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
        var dto = new UpdateBrandDto("Toyota", Guid.NewGuid(), "https://cdn.example.com/toyota.png");
        var controller = new BrandsController(service.Object);

        var actionResult = await controller.Update(id, dto, CancellationToken.None);

        var ok = actionResult as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(id));
        service.Verify(s => s.UpdateAsync(id, dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UploadLogo_WhenValidFileProvided_ShouldReturnOkWithUpdatedBrand()
    {
        var service = new Mock<IBrandService>();
        var id = Guid.NewGuid();
        var expected = new BrandResponseDto(id, "Toyota", Guid.NewGuid(), "Japan", "https://cdn.example.com/toyota.png");
        await using var fileStream = new MemoryStream([1, 2, 3]);
        var file = new FormFile(fileStream, 0, fileStream.Length, "file", "toyota.png");
        var request = new UploadBrandLogoRequest { File = file };

        service.Setup(s => s.UploadLogoAsync(id, It.IsAny<Stream>(), "toyota.png", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var controller = new BrandsController(service.Object);

        var actionResult = await controller.UploadLogo(id, request, CancellationToken.None);

        var ok = actionResult as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(expected));
        service.Verify(s => s.UploadLogoAsync(id, It.IsAny<Stream>(), "toyota.png", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UploadLogo_WhenExtensionInvalid_ShouldReturnBadRequest()
    {
        var service = new Mock<IBrandService>();
        var controller = new BrandsController(service.Object);
        await using var fileStream = new MemoryStream([1, 2, 3]);
        var file = new FormFile(fileStream, 0, fileStream.Length, "file", "toyota.exe");

        var actionResult = await controller.UploadLogo(Guid.NewGuid(), new UploadBrandLogoRequest { File = file }, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
        service.Verify(s => s.UploadLogoAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task UploadLogo_WhenExtensionUppercaseAndAllowed_ShouldCallService()
    {
        var service = new Mock<IBrandService>();
        var id = Guid.NewGuid();
        var expected = new BrandResponseDto(id, "Toyota", Guid.NewGuid(), "Japan", "https://cdn.example.com/toyota.png");
        await using var fileStream = new MemoryStream([1, 2, 3]);
        var file = new FormFile(fileStream, 0, fileStream.Length, "file", "toyota.PNG");

        service.Setup(s => s.UploadLogoAsync(id, It.IsAny<Stream>(), "toyota.PNG", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var controller = new BrandsController(service.Object);

        var actionResult = await controller.UploadLogo(id, new UploadBrandLogoRequest { File = file }, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<OkObjectResult>());
        service.Verify(s => s.UploadLogoAsync(id, It.IsAny<Stream>(), "toyota.PNG", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UploadLogo_WhenFileMissing_ShouldReturnBadRequest()
    {
        var service = new Mock<IBrandService>();
        var controller = new BrandsController(service.Object);

        var actionResult = await controller.UploadLogo(Guid.NewGuid(), new UploadBrandLogoRequest { File = null }, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
        service.Verify(s => s.UploadLogoAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}