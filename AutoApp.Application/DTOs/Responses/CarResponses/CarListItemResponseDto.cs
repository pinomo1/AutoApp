namespace AutoApp.Application.DTOs.Responses.CarResponses;

/// <summary>
/// Car list item response payload
/// </summary>
/// <param name="Id">GUID of the car</param>
/// <param name="BrandName">Brand name</param>
/// <param name="Model">Car model name</param>
/// <param name="Year">Production year</param>
/// <param name="Color">Body color</param>
/// <param name="Horsepower">Engine power in horsepower (hp)</param>
/// <param name="EngineVolumeCc">Engine displacement in cubic centimeters (cc)</param>
/// <param name="Price">Listing price</param>
/// <param name="Mileage">Vehicle mileage</param>
/// <param name="MainPhotoUrl">Main photo URL or storage path</param>
public record CarListItemResponseDto(
	Guid Id,
	string BrandName,
	string Model,
	short Year,
	string Color,
	int Horsepower,
	int EngineVolumeCc,
	decimal Price,
	double Mileage,
	string MainPhotoUrl);
