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
            LogoUrl = string.IsNullOrWhiteSpace(dto.LogoUrl) ? null : dto.LogoUrl.Trim()
        };
    }
    
    extension(UpdateBrandDto dto)
    {
        /// <summary>
        /// Maps an update-brand request DTO to a brand entity
        /// </summary>
        /// <param name="id">Brand identifier from route</param>
        /// <returns>Brand entity with updated values</returns>
        public Brand ToEntity(Guid id) => new()
        {
            Id = id,
            BrandName = dto.BrandName.Trim(),
            CountryId = dto.CountryId,
            LogoUrl = string.IsNullOrWhiteSpace(dto.LogoUrl) ? null : dto.LogoUrl.Trim()
        };
    }
}