using AutoApp.API.Controllers;
using AutoApp.Application.DTOs.Queries.FeatureQueries;
using AutoApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoApp.API.UnitTests.Controllers;

public class FeaturesControllerTests
{
    [Test]
    public async Task Create_WhenCalled_ShouldReturnCreatedAtActionWithId()
    {
        var service = new Mock<IFeatureService>();
        var dto = new CreateFeatureDto("Sunroof");
        var id = Guid.NewGuid();

        service.Setup(s => s.CreateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(id);
        var controller = new FeaturesController(service.Object);

        var actionResult = await controller.Create(dto, CancellationToken.None);

        var created = actionResult as CreatedAtActionResult;
        Assert.That(created, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(created!.ActionName, Is.EqualTo(nameof(FeaturesController.GetById)));
            Assert.That(created.Value, Is.EqualTo(id));
        });
        service.Verify(s => s.CreateAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Delete_WhenCalled_ShouldReturnNoContent()
    {
        var service = new Mock<IFeatureService>();
        var id = Guid.NewGuid();
        var controller = new FeaturesController(service.Object);

        var actionResult = await controller.Delete(id, CancellationToken.None);

        Assert.That(actionResult, Is.TypeOf<NoContentResult>());
        service.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}