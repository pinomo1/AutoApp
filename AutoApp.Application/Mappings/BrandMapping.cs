using AutoApp.Application.DTOs.Queries.BrandQueries;
using AutoApp.Application.DTOs.Responses.BrandResponses;
using AutoApp.Domain.Entities;

namespace AutoApp.Application.Mappings;

/// <summary>
/// Mapping helpers for brand entities and DTOs
/// </summary>
public static class BrandMapping
{
    extension(Brand brand)
    {
        /// <summary>
        /// Maps a brand entity to a response DTO
        /// </summary>
        /// <returns>Brand response DTO</returns>
        public BrandResponseDto ToDto() => new(brand.Id, brand.BrandName, brand.CountryId, brand.Country.CountryName, brand.LogoUrl);
    }

    extension(CreateBrandDto dto)
    {
        /// <summary>
        /// Maps a create-brand request DTO to a new brand entity
        /// </summary>
        /// <returns>Brand entity ready to be persisted</returns>
        public Brand ToEntity() => new()
        {
            Id = Guid.Empty,
            BrandName = dto.BrandName.Trim(),
            CountryId = dto.CountryId,
            LogoUrl = null
        };
    }

    extension(UpdateBrandDto dto)
    {
        /// <summary>
        /// Maps an update-brand request DTO to a brand entity
        /// </summary>
        /// <param name="id">Brand identifier from route</param>
        /// <param name="logoUrl">Url of a logo</param>
        /// <returns>Brand entity with updated values</returns>
        public Brand ToEntity(Guid id, string? logoUrl) => new()
        {
            Id = id,
            BrandName = dto.BrandName.Trim(),
            CountryId = dto.CountryId,
            LogoUrl = logoUrl
        };
    }
}
