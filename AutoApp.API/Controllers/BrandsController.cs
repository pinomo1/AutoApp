using AutoApp.Application.DTOs.Queries.BrandQueries;
using AutoApp.Application.DTOs.Responses.BrandResponses;
using AutoApp.Application.DTOs.Responses.SharedResponses;
using AutoApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers;

/// <summary>
/// Controller for car brands
/// </summary>
/// <param name="brandService">Brand service</param>
[ApiController]
[Route("api/[controller]")]
public class BrandsController(IBrandService brandService) : ControllerBase
{
    /// <summary>
    /// Search brands by name and/or country
    /// </summary>
    /// <param name="dto">DTO: Brand name and country ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Array of brands with total count by that search query</returns>
    [HttpGet]
    [ProducesResponseType(typeof(CountedResult<BrandResponseDto>),  StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] BrandSearchDto dto,
        CancellationToken ct)
        => Ok(await brandService.SearchAsync(dto, ct));
    
    /// <summary>
    /// Get brand by id
    /// </summary>
    /// <param name="id">Brand GUID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Country or 404</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BrandResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await brandService.GetByIdAsync(id, ct));
    
    /// <summary>
    /// Create new brand by name and country
    /// </summary>
    /// <param name="dto">DTO: Brand name and country ID</param>
    /// <param name="ct"></param>
    /// <returns>New ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateBrandDto dto, CancellationToken ct)
    {
        var id = await brandService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
    
    /// <summary>
    /// Update existing brand by ID with new name and/or country
    /// </summary>
    /// <param name="id">GUID of an existing brand</param>
    /// <param name="dto">DTO: New brand name and/or country ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>200 with ID</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateBrandDto dto, CancellationToken ct)
    {
        await brandService.UpdateAsync(id, dto, ct);
        return Ok(id);
    }
    
    /// <summary>
    /// Deletes the brand by its GUID
    /// </summary>
    /// <param name="id">GUID of the brand</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Nothing</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await brandService.DeleteAsync(id, ct);
        return NoContent();
    }
}