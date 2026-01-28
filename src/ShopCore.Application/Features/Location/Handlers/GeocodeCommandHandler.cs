using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Commands.Geocode;

public class GeocodeCommandHandler : IRequestHandler<GeocodeCommand, GeocodeResultDto>
{
    public Task<GeocodeResultDto> Handle(GeocodeCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
