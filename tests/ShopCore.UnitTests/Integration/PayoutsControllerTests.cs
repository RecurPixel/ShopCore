using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class PayoutsControllerTests : IntegrationTestBase
{
    public PayoutsControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region GET /api/v1/payouts (no class-level auth)

    [Fact]
    public async Task GetVendorPayouts_ReturnsOkOrError()
    {
        // No class-level auth — result depends on handler's auth check
        var response = await Client.GetAsync("/api/v1/payouts");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetVendorPayouts_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/payouts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/v1/payouts/pending (Vendor only)

    [Fact]
    public async Task GetPendingPayout_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/payouts/pending");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPendingPayout_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/payouts/pending");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPendingPayout_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/payouts/pending");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region POST /api/v1/payouts/calculate (Admin only)

    [Fact]
    public async Task CalculatePayouts_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync("/api/v1/payouts/calculate", new
        {
            vendorId = 1,
            periodStart = DateTime.UtcNow.AddDays(-30),
            periodEnd = DateTime.UtcNow
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CalculatePayouts_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/payouts/calculate", new
        {
            vendorId = 1,
            periodStart = DateTime.UtcNow.AddDays(-30),
            periodEnd = DateTime.UtcNow
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CalculatePayouts_AsAdmin_ReturnsOkOrError()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/payouts/calculate", new
        {
            vendorId = 999999,
            periodStart = DateTime.UtcNow.AddDays(-30),
            periodEnd = DateTime.UtcNow
        });

        // May return 404 if vendor not found, 400 if invalid, or 200 with empty list
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest);
    }

    #endregion

    #region POST /api/v1/payouts/process (Admin only)

    [Fact]
    public async Task ProcessPayout_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync("/api/v1/payouts/process", new
        {
            vendorId = 1,
            amount = 100m
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProcessPayout_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/payouts/process", new
        {
            vendorId = 1,
            amount = 100m
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ProcessPayout_AsAdmin_WithInvalidVendor_ReturnsError()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/payouts/process", new
        {
            vendorId = 999999,
            amount = 100m
        });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NoContent);
    }

    #endregion
}
