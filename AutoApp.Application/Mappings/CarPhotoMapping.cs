using AutoApp.Application.DTOs.Responses.CarPhotoResponses;
using AutoApp.Domain.Entities;

namespace AutoApp.Application.Mappings;

/// <summary>
/// Mapping helpers for car photo entities and DTOs
/// </summary>
public static class CarPhotoMapping
{
    extension(CarPhoto photo)
    {
        /// <summary>
        /// Maps a car photo entity to a response DTO
        /// </summary>
        /// <returns>Car photo response DTO</returns>
        public CarPhotoResponseDto ToDto() => new(photo.Id, photo.CarId, photo.PhotoUrl,
            photo.DisplayOrder, photo.IsMainPhoto);
    }
}
