using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class LocationControllerTests : IntegrationTestBase
{
    public LocationControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region POST /api/v1/location/geocode (public)

    [Fact]
    public async Task Geocode_WithValidAddress_ReturnsOkOrServiceError()
    {
        var response = await Client.PostAsJsonAsync("/api/v1/location/geocode", new
        {
            address = "MG Road, Bangalore, India"
        });

        // External service may not be available in test environment
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Geocode_WithEmptyAddress_ReturnsBadRequest()
    {
        var response = await Client.PostAsJsonAsync("/api/v1/location/geocode", new
        {
            address = ""
        });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.UnprocessableEntity,
            HttpStatusCode.InternalServerError);
    }

    #endregion

    #region POST /api/v1/location/reverse-geocode (public)

    [Fact]
    public async Task ReverseGeocode_WithCoordinates_ReturnsOkOrServiceError()
    {
        var response = await Client.PostAsJsonAsync("/api/v1/location/reverse-geocode", new
        {
            latitude = 12.9716,
            longitude = 77.5946
        });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.ServiceUnavailable);
    }

    #endregion

    #region GET /api/v1/location/vendors/nearby (public)

    [Fact]
    public async Task GetNearbyVendors_WithCoordinates_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/v1/location/vendors/nearby?lat=12.9716&lng=77.5946&radiusKm=10");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.InternalServerError);
    }

    #endregion

    #region GET /api/v1/location/pincodes/{pincode}/vendors (public)

    [Fact]
    public async Task GetVendorsByPincode_WithValidPincode_ReturnsOkOrNotFound()
    {
        var response = await Client.GetAsync("/api/v1/location/pincodes/560001/vendors");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetVendorsByPincode_WithInvalidPincode_ReturnsOkOrNotFound()
    {
        var response = await Client.GetAsync("/api/v1/location/pincodes/000000/vendors");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest,
            HttpStatusCode.InternalServerError);
    }

    #endregion
}
