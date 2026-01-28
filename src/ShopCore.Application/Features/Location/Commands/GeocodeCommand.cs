using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Commands.Geocode;

public record GeocodeCommand(string Address) : IRequest<GeocodeResultDto>;
