using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Application.DTOs.Responses.CarResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers;

/// <summary>
/// Controller for cars
/// </summary>
/// <param name="carService">Car service</param>
[ApiController]
[Route("api/[controller]")]
public class CarsController(ICarService carService) : ControllerBase
{
    /// <summary>
    /// Search cars by filters and pagination
    /// </summary>
    /// <param name="dto">DTO: Search filters and pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated cars by that search query</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<CarListItemResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] CarSearchDto dto,
        CancellationToken ct)
        => Ok(await carService.SearchAsync(dto, ct));

    /// <summary>
    /// Get car by id
    /// </summary>
    /// <param name="id">Car GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Car or 404</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CarDetailsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await carService.GetByIdAsync(id, ct));

    /// <summary>
    /// Create new car
    /// </summary>
    /// <param name="dto">DTO: Car data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>New ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateCarDto dto, CancellationToken ct)
    {
        var id = await carService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Update existing car by ID
    /// </summary>
    /// <param name="id">GUID of an existing car</param>
    /// <param name="dto">DTO: New car values</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>200 with ID</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateCarDto dto, CancellationToken ct)
    {
        await carService.UpdateAsync(id, dto, ct);
        return Ok(id);
    }

    /// <summary>
    /// Adds a feature to the car
    /// </summary>
    /// <param name="id">GUID of the car</param>
    /// <param name="featureId">GUID of the feature</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Nothing</returns>
    [HttpPost("{id:guid}/features/{featureId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddFeature(Guid id, Guid featureId, CancellationToken ct)
    {
        await carService.AddFeatureAsync(id, featureId, ct);
        return NoContent();
    }

    /// <summary>
    /// Removes a feature from the car
    /// </summary>
    /// <param name="id">GUID of the car</param>
    /// <param name="featureId">GUID of the feature</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Nothing</returns>
    [HttpDelete("{id:guid}/features/{featureId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFeature(Guid id, Guid featureId, CancellationToken ct)
    {
        await carService.RemoveFeatureAsync(id, featureId, ct);
        return NoContent();
    }

    /// <summary>
    /// Deletes the car by its GUID
    /// </summary>
    /// <param name="id">GUID of the car</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Nothing</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await carService.DeleteAsync(id, ct);
        return NoContent();
    }
}