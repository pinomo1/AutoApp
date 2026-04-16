using AutoApp.Application.DTOs.Queries.FeatureQueries;
using AutoApp.Application.DTOs.Responses.FeatureResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers;

/// <summary>
/// Controller for features
/// </summary>
/// <param name="featureService">Feature service</param>
[ApiController]
[Route("api/[controller]")]
public class FeaturesController(IFeatureService featureService) : ControllerBase
{
    /// <summary>
    /// Search features by name
    /// </summary>
    /// <param name="dto">DTO: Feature name</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Array of features with total count by that search query</returns>
    [HttpGet]
    [ProducesResponseType(typeof(CountedResult<FeatureResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] FeatureSearchDto dto,
        CancellationToken ct)
        => Ok(await featureService.SearchAsync(dto, ct));

    /// <summary>
    /// Get feature by id
    /// </summary>
    /// <param name="id">Feature GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Feature or 404</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FeatureResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await featureService.GetByIdAsync(id, ct));

    /// <summary>
    /// Create new feature by name
    /// </summary>
    /// <param name="dto">DTO: Feature name</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>New ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateFeatureDto dto, CancellationToken ct)
    {
        var id = await featureService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Update existing feature by ID with new name
    /// </summary>
    /// <param name="id">GUID of an existing feature</param>
    /// <param name="dto">DTO: New feature name</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>200 with ID</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateFeatureDto dto, CancellationToken ct)
    {
        await featureService.UpdateAsync(id, dto, ct);
        return Ok(id);
    }

    /// <summary>
    /// Deletes the feature by its GUID
    /// </summary>
    /// <param name="id">GUID of the feature</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Nothing</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await featureService.DeleteAsync(id, ct);
        return NoContent();
    }
}