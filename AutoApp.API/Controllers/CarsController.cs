using AutoApp.Application.DTOs.Queries;
using AutoApp.Application.DTOs.Responses;
using AutoApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers;

/// <summary>
/// Controller for cars
/// </summary>
/// <param name="carService">Service that has methods to interact with cars in database</param>
[ApiController]
[Route("api/[controller]")]
public class CarsController(ICarService carService) : ControllerBase
{
    /// <summary>
    /// Get the cars with pagination and filtering
    /// </summary>
    /// <param name="query">Pagination settings</param>
    /// <param name="filters">Filter settings</param>
    /// <param name="sorting">Sorting settings</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Collection of cars</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<ResponseCarDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginatedQuery query, 
        [FromQuery] CarFilters filters,
        [FromQuery] CarSorting sorting,
        CancellationToken ct)
        => Ok(await carService.GetAllAsync(query, filters, sorting, ct));

    /// <summary>
    /// Gets one car by its unique GUID
    /// </summary>
    /// <param name="id">ID of the car</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Car with that ID</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ResponseCarDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await carService.GetByIdAsync(id, ct));

    /// <summary>
    /// Creates a car
    /// </summary>
    /// <param name="dto">Car DTO</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>201 if created successfully</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateCarDto dto, CancellationToken ct)
    {
        var id = await carService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Updates the car info by ID
    /// </summary>
    /// <param name="id">ID of the car</param>
    /// <param name="dto">New info</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Nothing</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateCarDto dto, CancellationToken ct)
    {
        await carService.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Deletes the car by ID
    /// </summary>
    /// <param name="id">ID of the car</param>
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