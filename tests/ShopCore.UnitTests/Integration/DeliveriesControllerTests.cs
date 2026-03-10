using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;

namespace ShopCore.UnitTests.Integration;

public class DeliveriesControllerTests : IntegrationTestBase
{
    public DeliveriesControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region GET /api/v1/deliveries/{id} (auth required)

    [Fact]
    public async Task GetDeliveryById_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/deliveries/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDeliveryById_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/deliveries/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDeliveryById_AsVendor_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/deliveries/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/v1/deliveries/{id}/download-receipt (auth required)

    [Fact]
    public async Task DownloadDeliveryReceipt_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/deliveries/1/download-receipt");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DownloadDeliveryReceipt_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/deliveries/999999/download-receipt");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
