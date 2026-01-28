using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Commands.ReverseGeocode;

public record ReverseGeocodeCommand(
    double Latitude,
    double Longitude
) : IRequest<ReverseGeocodeResultDto>;
