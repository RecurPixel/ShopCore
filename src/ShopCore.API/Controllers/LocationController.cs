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

    /// <summary>
    /// Creates or processes geocode.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>GeocodeResultDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPost("geocode")]
    public async Task<ActionResult<GeocodeResultDto>> Geocode(
        [FromBody] GeocodeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Creates or processes reverse geocode.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>ReverseGeocodeResultDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPost("reverse-geocode")]
    public async Task<ActionResult<ReverseGeocodeResultDto>> ReverseGeocode(
        [FromBody] ReverseGeocodeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves nearby vendors.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>List&lt;NearbyVendorDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("vendors/nearby")]
    public async Task<ActionResult<List<NearbyVendorDto>>> GetNearbyVendors(
        [FromQuery] GetNearbyVendorsQuery query)
    {
        var vendors = await _mediator.Send(query);
        return Ok(vendors);
    }

    /// <summary>
    /// Retrieves vendors by pincode.
    /// </summary>
    /// <param name="pincode">The pincode</param>
    /// <returns>List&lt;NearbyVendorDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("pincodes/{pincode}/vendors")]
    public async Task<ActionResult<List<NearbyVendorDto>>> GetVendorsByPincode(string pincode)
    {
        var vendors = await _mediator.Send(new GetVendorsByPincodeQuery(pincode));
        return Ok(vendors);
    }
}
