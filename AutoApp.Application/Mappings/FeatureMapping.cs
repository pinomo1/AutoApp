using AutoApp.Application.DTOs.Queries.FeatureQueries;
using AutoApp.Application.DTOs.Responses.FeatureResponses;
using AutoApp.Domain.Entities;

namespace AutoApp.Application.Mappings;

/// <summary>
/// Mapping helpers for feature entities and DTOs
/// </summary>
public static class FeatureMapping
{
    extension(Feature feature)
    {
        /// <summary>
        /// Maps a feature entity to a response DTO
        /// </summary>
        /// <returns>Feature response DTO</returns>
        public FeatureResponseDto ToDto() => new(feature.Id, feature.FeatureName);
    }

    extension(CreateFeatureDto dto)
    {
        /// <summary>
        /// Maps a create-feature request DTO to a new feature entity
        /// </summary>
        /// <returns>Feature entity ready to be persisted</returns>
        public Feature ToEntity() => new()
        {
            Id = Guid.Empty,
            FeatureName = dto.FeatureName.Trim()
        };
    }

    extension(UpdateFeatureDto dto)
    {
        /// <summary>
        /// Maps an update-feature request DTO to a feature entity
        /// </summary>
        /// <param name="id">Feature identifier from route</param>
        /// <returns>Feature entity with updated values</returns>
        public Feature ToEntity(Guid id) => new()
        {
            Id = id,
            FeatureName = dto.FeatureName.Trim()
        };
    }
}