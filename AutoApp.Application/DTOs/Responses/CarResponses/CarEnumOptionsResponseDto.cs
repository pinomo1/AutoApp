namespace AutoApp.Application.DTOs.Responses.CarResponses;

/// <summary>
/// Available enum options for car-related fields
/// </summary>
/// <param name="CarConditions">Available car conditions</param>
/// <param name="CarTypes">Available car types</param>
/// <param name="FuelTypes">Available fuel types</param>
/// <param name="TransmissionTypes">Available transmission types</param>
/// <param name="Colors">Available colors</param>
/// <param name="CarSortTypes">Available car sorting types</param>
public record CarEnumOptionsResponseDto(
    string[] CarConditions,
    string[] CarTypes,
    string[] FuelTypes,
    string[] TransmissionTypes,
    string[] Colors,
    string[] CarSortTypes);