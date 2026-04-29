using AutoApp.Application.DTOs.Responses.CarPhotoResponses;
using AutoApp.Application.Exceptions;
using AutoApp.Application.Mappings;
using AutoApp.Application.Services.Interfaces;
using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Persistence.DbContexts;
using AutoApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.Services;

/// <summary>
/// Service for car photo CRUD operations and search
/// </summary>
/// <param name="db">Database context abstraction</param>
/// <param name="carPhotoStorage">Car photo storage abstraction</param>
public class CarPhotoService(IAutoDbContext db, ICarPhotoStorage carPhotoStorage) : ICarPhotoService
{
    /// <summary>
    /// Gets all photos for a specific car
    /// </summary>
    public async Task<List<CarPhotoResponseDto>> GetByCarIdAsync(Guid carId, CancellationToken ct)
    {
        if (!await db.Cars.AnyAsync(c => c.Id == carId, ct))
            throw new NotFoundException(nameof(Car), carId);

        var photos = await db.CarPhotos
            .Where(p => p.CarId == carId)
            .OrderBy(p => p.DisplayOrder)
            .AsNoTracking()
            .Select(p => p.ToDto())
            .ToListAsync(ct);

        return photos;
    }

    /// <summary>
    /// Gets a car photo by identifier
    /// </summary>
    public async Task<CarPhotoResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var photo = await db.CarPhotos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return photo?.ToDto() ?? throw new NotFoundException(nameof(CarPhoto), id);
    }

    /// <summary>
    /// Creates a new car photo
    /// </summary>
    public async Task<Guid> CreateAsync(Guid carId, Stream content, string fileName, int displayOrder, bool isMainPhoto, CancellationToken ct)
    {
        var car = await db.Cars.FirstOrDefaultAsync(c => c.Id == carId, ct);
        if (car == null)
            throw new NotFoundException(nameof(Car), carId);

        var photoId = Guid.NewGuid();
        var photoUrl = await carPhotoStorage.UploadAsync(photoId, content, fileName, ct);

        var photo = new CarPhoto
        {
            Id = photoId,
            CarId = carId,
            Car = car,
            PhotoUrl = photoUrl.Trim(),
            DisplayOrder = displayOrder,
            IsMainPhoto = isMainPhoto
        };

        // If this is the main photo, unset main photo flag on other photos
        if (photo.IsMainPhoto)
        {
            var existingPhotos = await db.CarPhotos
                .Where(p => p.CarId == carId && p.IsMainPhoto)
                .ToListAsync(ct);

            foreach (var existingPhoto in existingPhotos)
            {
                existingPhoto.IsMainPhoto = false;
            }
        }

        db.CarPhotos.Add(photo);
        await db.SaveChangesAsync(ct);
        return photo.Id;
    }

    /// <summary>
    /// Updates an existing car photo
    /// </summary>
    public async Task UpdateAsync(Guid carId, Guid id, Stream content, string fileName, int displayOrder, bool isMainPhoto, CancellationToken ct)
    {
        var existingPhoto = await db.CarPhotos.FirstOrDefaultAsync(p => p.Id == id && p.CarId == carId, ct);
        if (existingPhoto == null)
            throw new NotFoundException(nameof(CarPhoto), id);

        // If this is the main photo, unset main photo flag on other photos
        if (isMainPhoto && !existingPhoto.IsMainPhoto)
        {
            var mainPhotos = await db.CarPhotos
                .Where(p => p.CarId == existingPhoto.CarId && p.IsMainPhoto && p.Id != id)
                .ToListAsync(ct);

            foreach (var mainPhoto in mainPhotos)
            {
                mainPhoto.IsMainPhoto = false;
            }
        }

        var photoUrl = await carPhotoStorage.UploadAsync(id, content, fileName, ct);

        existingPhoto.PhotoUrl = photoUrl.Trim();
        existingPhoto.DisplayOrder = displayOrder;
        existingPhoto.IsMainPhoto = isMainPhoto;

        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Deletes a car photo by identifier
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var photo = await db.CarPhotos.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (photo == null)
            throw new NotFoundException(nameof(CarPhoto), id);

        db.CarPhotos.Remove(photo);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Deletes all photos for a specific car
    /// </summary>
    public async Task DeleteByCarIdAsync(Guid carId, CancellationToken ct)
    {
        if (!await db.Cars.AnyAsync(c => c.Id == carId, ct))
            throw new NotFoundException(nameof(Car), carId);

        var photos = await db.CarPhotos
            .Where(p => p.CarId == carId)
            .ToListAsync(ct);

        db.CarPhotos.RemoveRange(photos);
        await db.SaveChangesAsync(ct);
    }
}
