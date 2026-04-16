using AutoApp.Application.DTOs.Queries.CountryQueries;
using AutoApp.Application.DTOs.Responses.CountryResponses;
using AutoApp.Domain.Entities;

namespace AutoApp.Application.Mappings;

/// <summary>
/// Mapping helpers for country entities and DTOs
/// </summary>
public static class CountryMapping
{
    extension(Country country)
    {
        /// <summary>
        /// Maps a country entity to a response DTO
        /// </summary>
        /// <returns>Country response DTO</returns>
        public CountryResponseDto ToDto() => new(country.Id, country.CountryName);
    }

    extension(CreateCountryDto dto)
    {
        /// <summary>
        /// Maps a create-country request DTO to a new country entity
        /// </summary>
        /// <returns>Country entity ready to be persisted</returns>
        public Country ToEntity() => new()
        {
            Id = Guid.Empty,
            CountryName = dto.CountryName.Trim()
        };
    }

    extension(UpdateCountryDto dto)
    {
        /// <summary>
        /// Maps an update-country request DTO to a country entity
        /// </summary>
        /// <param name="id">Country identifier from route</param>
        /// <returns>Country entity with updated values</returns>
        public Country ToEntity(Guid id) => new()
        {
            Id = id,
            CountryName = dto.CountryName.Trim()
        };
    }
}