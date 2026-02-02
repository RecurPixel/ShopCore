using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Commands.Geocode;

public class GeocodeCommandHandler : IRequestHandler<GeocodeCommand, GeocodeResultDto>
{
    private readonly ILocationService _locationService;

    public GeocodeCommandHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<GeocodeResultDto> Handle(GeocodeCommand request, CancellationToken ct)
    {
        var result = await _locationService.GeocodeAddressAsync(request.Address);

        if (result == null || !result.IsSuccess)
            throw new ValidationException("Unable to geocode address");

        return new GeocodeResultDto
        {
            FormattedAddress = result.FormattedAddress,
            Latitude = result.Latitude,
            Longitude = result.Longitude,
            PlaceId = result.PlaceId,
            City = result.City,
            State = result.State,
            Country = result.Country,
            Pincode = result.Pincode
        };
    }
}
