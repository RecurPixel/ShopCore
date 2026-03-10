using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class VendorsControllerTests : IntegrationTestBase
{
    public VendorsControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    private record VendorDto(int Id, string BusinessName);
    private record ProductDto(int Id, string Name, decimal Price);

    #region GET /api/v1/vendors/search (public)

    [Fact]
    public async Task SearchVendorsByLocation_Anonymous_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/v1/vendors/search?lat=12.9716&lng=77.5946&radiusKm=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/v1/vendors/{id} (public)

    [Fact]
    public async Task GetVendorById_WithInvalidId_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/v1/vendors/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/v1/vendors/{id}/products (public)

    [Fact]
    public async Task GetVendorPublicProducts_WithInvalidVendorId_ReturnsOkWithEmptyOrNotFound()
    {
        var response = await Client.GetAsync("/api/v1/vendors/999999/products");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/v1/vendors/register (auth required)

    [Fact]
    public async Task RegisterVendor_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync("/api/v1/vendors/register", new
        {
            businessName = "My Shop",
            businessEmail = "myshop@test.com",
            businessPhone = "9876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region GET /api/v1/vendors/me (Vendor only)

    [Fact]
    public async Task GetMyVendorProfile_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/vendors/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyVendorProfile_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMyVendorProfile_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/v1/vendors/me/stats (Vendor only)

    [Fact]
    public async Task GetVendorStats_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/stats");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetVendorStats_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/stats");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/v1/vendors/me/products (Vendor only)

    [Fact]
    public async Task GetMyProducts_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMyProducts_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/products");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region GET /api/v1/vendors/me/orders (Vendor only)

    [Fact]
    public async Task GetVendorOrders_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/orders");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetVendorOrders_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/vendors/me/orders");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region GET /api/v1/vendors/me/subscriptions (Vendor only)

    [Fact]
    public async Task GetVendorSubscriptions_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/subscriptions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/v1/vendors/me/deliveries (Vendor only)

    [Fact]
    public async Task GetVendorDeliveries_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/deliveries");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/v1/vendors/me/invoices (Vendor only)

    [Fact]
    public async Task GetVendorInvoices_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/invoices");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/v1/vendors/me/payouts (Vendor only)

    [Fact]
    public async Task GetMyPayouts_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/payouts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPendingPayoutAmount_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/payouts/pending");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/v1/vendors/me/customers (Vendor only)

    [Fact]
    public async Task GetVendorCustomers_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetVendorCustomerById_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/customers/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/v1/vendors/me/invitations (Vendor only)

    [Fact]
    public async Task GetMyInvitations_AsVendor_ReturnsOk()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/invitations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMyInvitations_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/vendors/me/invitations");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion
}
