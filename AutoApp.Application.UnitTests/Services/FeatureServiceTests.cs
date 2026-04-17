using AutoApp.Application.Exceptions;
using AutoApp.Application.Services;
using AutoApp.Application.UnitTests.Common;
using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.UnitTests.Services;

public class FeatureServiceTests
{
    [Test]
    public async Task GetByIdAsync_WhenFeatureExists_ShouldReturnMappedDto()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var feature = new Feature { Id = Guid.NewGuid(), FeatureName = "ABS" };
        dbContext.Features.Add(feature);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new FeatureService(dbContext);
        var dto = await service.GetByIdAsync(feature.Id, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(feature.Id));
            Assert.That(dto.FeatureName, Is.EqualTo("ABS"));
        });
    }

    [Test]
    public void GetByIdAsync_WhenFeatureDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new FeatureService(dbContext);

        Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Test]
    public async Task DeleteAsync_WhenFeatureExists_ShouldRemoveFeature()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var feature = new Feature { Id = Guid.NewGuid(), FeatureName = "Lane Assist" };
        dbContext.Features.Add(feature);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new FeatureService(dbContext);
        await service.DeleteAsync(feature.Id, CancellationToken.None);

        var exists = await dbContext.Features.AnyAsync(f => f.Id == feature.Id);
        Assert.That(exists, Is.False);
    }
}