using AutoApp.Application.DTOs.Queries.CountryQueries;
using AutoApp.Application.DTOs.Responses.CountryResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers;

/// <summary>
/// Controller for countries
/// </summary>
/// <param name="countryService">Country service</param>
[ApiController]
[Route("api/[controller]")]
public class CountriesController(ICountryService countryService) : ControllerBase
{
    /// <summary>
    /// Search countries by name
    /// </summary>
    /// <param name="dto">DTO: Country name</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Array of countries with total count by that search query</returns>
    [HttpGet]
    [ProducesResponseType(typeof(CountedResult<CountryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] CountrySearchDto dto,
        CancellationToken ct)
        => Ok(await countryService.SearchAsync(dto, ct));

    /// <summary>
    /// Get country by id
    /// </summary>
    /// <param name="id">Country GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Country or 404</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CountryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await countryService.GetByIdAsync(id, ct));

    /// <summary>
    /// Create new country by name
    /// </summary>
    /// <param name="dto">DTO: Country name</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>New ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateCountryDto dto, CancellationToken ct)
    {
        var id = await countryService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Update existing country by ID with new name
    /// </summary>
    /// <param name="id">GUID of an existing country</param>
    /// <param name="dto">DTO: New country name</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>200 with ID</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateCountryDto dto, CancellationToken ct)
    {
        await countryService.UpdateAsync(id, dto, ct);
        return Ok(id);
    }

    /// <summary>
    /// Deletes the country by its GUID
    /// </summary>
    /// <param name="id">GUID of the country</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Nothing</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await countryService.DeleteAsync(id, ct);
        return NoContent();
    }
}