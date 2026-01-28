using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Commands.ReverseGeocode;

public class ReverseGeocodeCommandHandler : IRequestHandler<ReverseGeocodeCommand, ReverseGeocodeResultDto>
{
    public Task<ReverseGeocodeResultDto> Handle(ReverseGeocodeCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
