using AutoApp.Application.DTOs.Responses.CarResponses;
using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Controllers;

/// <summary>
/// Controller for frontend metadata endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MetaController : ControllerBase
{
    /// <summary>
    /// Returns available enum values for car forms and filters
    /// </summary>
    /// <returns>Car enum options</returns>
    [HttpGet("car-enums")]
    [ProducesResponseType(typeof(CarEnumOptionsResponseDto), StatusCodes.Status200OK)]
    public IActionResult GetCarEnums()
        => Ok(new CarEnumOptionsResponseDto(
            Enum.GetNames<CarCondition>(),
            Enum.GetNames<CarType>(),
            Enum.GetNames<FuelType>(),
            Enum.GetNames<TransmissionType>(),
            Enum.GetNames<Color>(),
            Enum.GetNames<CarSortType>()));
}