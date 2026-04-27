using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Infrastructure.UnitTests.Persistence;

public class AutoDbContextModelConfigurationTests
{
    [Test]
    public void Model_WhenConfigured_ShouldUseCompositeKeyForCarFeature()
    {
        using var dbContext = CreateDbContext();
        var entityType = dbContext.Model.FindEntityType(typeof(CarFeature));

        Assert.That(entityType, Is.Not.Null);

        var primaryKey = entityType!.FindPrimaryKey();

        Assert.That(primaryKey, Is.Not.Null);
        Assert.That(primaryKey!.Properties.Select(p => p.Name), Is.EquivalentTo(new[] { nameof(CarFeature.CarId), nameof(CarFeature.FeatureId) }));
    }

    [Test]
    public void Model_WhenConfigured_ShouldApplyCarModelLengthAndType()
    {
        using var dbContext = CreateDbContext();
        var entityType = dbContext.Model.FindEntityType(typeof(Car));

        Assert.That(entityType, Is.Not.Null);

        var modelProperty = entityType!.FindProperty(nameof(Car.Model));

        Assert.That(modelProperty, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(modelProperty!.GetMaxLength(), Is.EqualTo(32));
            Assert.That(modelProperty.IsNullable, Is.False);
        });
    }

    [Test]
    public void Model_WhenConfigured_ShouldSetForeignKeysForCarAndBrand()
    {
        using var dbContext = CreateDbContext();

        var carEntityType = dbContext.Model.FindEntityType(typeof(Car));
        var brandEntityType = dbContext.Model.FindEntityType(typeof(Brand));

        Assert.Multiple(() =>
        {
            Assert.That(carEntityType, Is.Not.Null);
            Assert.That(brandEntityType, Is.Not.Null);
        });

        var carBrandForeignKey = carEntityType!.GetForeignKeys()
            .Single(fk => fk.Properties.Any(p => p.Name == nameof(Car.BrandId)));

        Assert.That(carBrandForeignKey.PrincipalEntityType.ClrType, Is.EqualTo(typeof(Brand)));

        var brandCountryForeignKey = brandEntityType!.GetForeignKeys()
            .Single(fk => fk.Properties.Any(p => p.Name == nameof(Brand.CountryId)));

        Assert.That(brandCountryForeignKey.PrincipalEntityType.ClrType, Is.EqualTo(typeof(Country)));
    }

    [Test]
    public void Model_WhenConfigured_ShouldApplyCountryCodeLengthAndType()
    {
        using var dbContext = CreateDbContext();
        var entityType = dbContext.Model.FindEntityType(typeof(Country));

        Assert.That(entityType, Is.Not.Null);

        var codeProperty = entityType!.FindProperty(nameof(Country.CountryCode));

        Assert.That(codeProperty, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(codeProperty!.GetMaxLength(), Is.EqualTo(2));
            Assert.That(codeProperty.IsNullable, Is.False);
        });
    }

    private static AutoDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AutoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AutoDbContext(options);
    }
}