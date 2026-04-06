using AutoApp.Application.DTOs;
using AutoApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController(ICarService carService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginatedQuery query, [FromQuery] CarFilters filters, CancellationToken ct)
        => Ok(await carService.GetAllAsync(query, filters, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await carService.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateCarDto dto, CancellationToken ct)
    {
        var id = await carService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCarDto dto, CancellationToken ct)
    {
        await carService.UpdateAsync(dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await carService.DeleteAsync(id, ct);
        return NoContent();
    }
}