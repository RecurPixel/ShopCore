using ShopCore.Application.Location.Commands.Geocode;
using ShopCore.Application.Location.Commands.ReverseGeocode;
using ShopCore.Application.Location.DTOs;
using ShopCore.Application.Location.Queries.GetNearbyVendors;
using ShopCore.Application.Location.Queries.GetVendorsByPincode;

namespace ShopCore.Api.Controllers;

/// <summary>
/// Location and geocoding services.
/// </summary>
[ApiController]
[Route("api/v1/location")]
public class LocationController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/v1/location/geocode
    [HttpPost("geocode")]
    public async Task<ActionResult<GeocodeResultDto>> Geocode(
        [FromBody] GeocodeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // POST /api/v1/location/reverse-geocode
    [HttpPost("reverse-geocode")]
    public async Task<ActionResult<ReverseGeocodeResultDto>> ReverseGeocode(
        [FromBody] ReverseGeocodeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // GET /api/v1/location/vendors/nearby
    [HttpGet("vendors/nearby")]
    public async Task<ActionResult<List<NearbyVendorDto>>> GetNearbyVendors(
        [FromQuery] GetNearbyVendorsQuery query)
    {
        var vendors = await _mediator.Send(query);
        return Ok(vendors);
    }

    // GET /api/v1/location/pincodes/{pincode}/vendors
    [HttpGet("pincodes/{pincode}/vendors")]
    public async Task<ActionResult<List<NearbyVendorDto>>> GetVendorsByPincode(string pincode)
    {
        var vendors = await _mediator.Send(new GetVendorsByPincodeQuery(pincode));
        return Ok(vendors);
    }
}
