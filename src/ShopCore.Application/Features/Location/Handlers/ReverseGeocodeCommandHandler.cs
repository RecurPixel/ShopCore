using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Commands.ReverseGeocode;

public class ReverseGeocodeCommandHandler : IRequestHandler<ReverseGeocodeCommand, ReverseGeocodeResultDto>
{
    private readonly ILocationService _locationService;

    public ReverseGeocodeCommandHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<ReverseGeocodeResultDto> Handle(ReverseGeocodeCommand request, CancellationToken ct)
    {
        var result = await _locationService.ReverseGeocodeAsync(request.Latitude, request.Longitude);

        if (result == null || !result.IsSuccess)
            throw new ValidationException("Unable to reverse geocode coordinates");

        return new ReverseGeocodeResultDto
        {
            FormattedAddress = result.FormattedAddress,
            City = result.City,
            State = result.State,
            Country = result.Country,
            Pincode = result.Pincode,
            PlaceId = result.PlaceId
        };
    }
}
}
