using AutoApp.API.Controllers;
using AutoApp.API.Controllers.Requests;
using AutoApp.Application.DTOs.Responses.CarPhotoResponses;
using AutoApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoApp.API.UnitTests.Controllers;

public class CarPhotosControllerTests
{
    [Test]
    public async Task GetByCarId_WhenCalled_ShouldReturnOkWithPhotos()
    {
        var service = new Mock<ICarPhotoService>();
        var carId = Guid.NewGuid();
        var photos = new List<CarPhotoResponseDto>
        {
            new(Guid.NewGuid(), carId, "http://example.com/photo1.jpg", 0, true),
            new(Guid.NewGuid(), carId, "http://example.com/photo2.jpg", 1, false)
        };

        service.Setup(s => s.GetByCarIdAsync(carId, It.IsAny<CancellationToken>())).ReturnsAsync(photos);
        var controller = new CarPhotosController(service.Object);

        var actionResult = await controller.GetByCarId(carId, CancellationToken.None);

        var okResult = actionResult as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            var returnedPhotos = okResult.Value as List<CarPhotoResponseDto>;
            Assert.That(returnedPhotos, Has.Count.EqualTo(2));
        });
        service.Verify(s => s.GetByCarIdAsync(carId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetById_WhenCalled_ShouldReturnOkWithPhoto()
    {
        var service = new Mock<ICarPhotoService>();
        var carId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var photo = new CarPhotoResponseDto(photoId, carId, "http://example.com/photo.jpg", 0, true);

        service.Setup(s => s.GetByIdAsync(photoId, It.IsAny<CancellationToken>())).ReturnsAsync(photo);
        var controller = new CarPhotosController(service.Object);

        var actionResult = await controller.GetById(carId, photoId, CancellationToken.None);

        var okResult = actionResult as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            var returnedPhoto = okResult.Value as CarPhotoResponseDto;
            Assert.That(returnedPhoto?.Id, Is.EqualTo(photoId));
        });
        service.Verify(s => s.GetByIdAsync(photoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Create_WhenCalled_ShouldReturnCreatedAtActionWithId()
    {
        var service = new Mock<ICarPhotoService>();
        var carId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var request = new UploadCarPhotoRequest { File = CreateFile("photo.jpg"), DisplayOrder = 0, IsMainPhoto = true };

        service.Setup(s => s.CreateAsync(carId, It.IsAny<Stream>(), "photo.jpg", 0, true, It.IsAny<CancellationToken>())).ReturnsAsync(photoId);
        var controller = new CarPhotosController(service.Object);

        var actionResult = await controller.Create(carId, request, CancellationToken.None);

        var createdResult = actionResult as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdResult!.ActionName, Is.EqualTo(nameof(CarPhotosController.GetById)));
            Assert.That(createdResult.Value, Is.EqualTo(photoId));
        });
        service.Verify(s => s.CreateAsync(carId, It.IsAny<Stream>(), "photo.jpg", 0, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Create_WhenExtensionInvalid_ShouldReturnBadRequest()
    {
        var service = new Mock<ICarPhotoService>();
        var request = new UploadCarPhotoRequest { File = CreateFile("photo.exe"), DisplayOrder = 0, IsMainPhoto = true };

        var controller = new CarPhotosController(service.Object);

        var actionResult = await controller.Create(Guid.NewGuid(), request, CancellationToken.None);

        var badRequestResult = actionResult as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task Create_WhenFileMissing_ShouldReturnBadRequest()
    {
        var service = new Mock<ICarPhotoService>();
        var controller = new CarPhotosController(service.Object);

        var actionResult = await controller.Create(Guid.NewGuid(), new UploadCarPhotoRequest { File = null, DisplayOrder = 0, IsMainPhoto = true }, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
        service.Verify(s => s.CreateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Update_WhenCalled_ShouldReturnOkWithId()
    {
        var service = new Mock<ICarPhotoService>();
        var carId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var request = new UploadCarPhotoRequest { File = CreateFile("updated.jpg"), DisplayOrder = 0, IsMainPhoto = true };

        service.Setup(s => s.UpdateAsync(carId, photoId, It.IsAny<Stream>(), "updated.jpg", 0, true, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var controller = new CarPhotosController(service.Object);

        var actionResult = await controller.Update(carId, photoId, request, CancellationToken.None);

        var okResult = actionResult as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(photoId));
        });
        service.Verify(s => s.UpdateAsync(carId, photoId, It.IsAny<Stream>(), "updated.jpg", 0, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Update_WhenExtensionUppercaseAndAllowed_ShouldCallService()
    {
        var service = new Mock<ICarPhotoService>();
        var carId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var request = new UploadCarPhotoRequest { File = CreateFile("UPDATED.PNG"), DisplayOrder = 1, IsMainPhoto = false };

        service.Setup(s => s.UpdateAsync(carId, photoId, It.IsAny<Stream>(), "UPDATED.PNG", 1, false, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var controller = new CarPhotosController(service.Object);

        var actionResult = await controller.Update(carId, photoId, request, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<OkObjectResult>());
        service.Verify(s => s.UpdateAsync(carId, photoId, It.IsAny<Stream>(), "UPDATED.PNG", 1, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Update_WhenFileMissing_ShouldReturnBadRequest()
    {
        var service = new Mock<ICarPhotoService>();
        var controller = new CarPhotosController(service.Object);

        var actionResult = await controller.Update(Guid.NewGuid(), Guid.NewGuid(), new UploadCarPhotoRequest { File = null, DisplayOrder = 0, IsMainPhoto = true }, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
        service.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Delete_WhenCalled_ShouldReturnNoContent()
    {
        var service = new Mock<ICarPhotoService>();
        var carId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var controller = new CarPhotosController(service.Object);

        var actionResult = await controller.Delete(carId, photoId, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<NoContentResult>());
        service.Verify(s => s.DeleteAsync(photoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteByCar_WhenCalled_ShouldReturnNoContent()
    {
        var service = new Mock<ICarPhotoService>();
        var carId = Guid.NewGuid();
        var controller = new CarPhotosController(service.Object);

        var actionResult = await controller.DeleteByCar(carId, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<NoContentResult>());
        service.Verify(s => s.DeleteByCarIdAsync(carId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static IFormFile CreateFile(string fileName)
    {
        var stream = new MemoryStream([1, 2, 3]);
        return new FormFile(stream, 0, stream.Length, "file", fileName);
    }
}
