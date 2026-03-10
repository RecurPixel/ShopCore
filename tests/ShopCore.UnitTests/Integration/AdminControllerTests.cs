using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class AdminControllerTests : IntegrationTestBase
{
    public AdminControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    private record UserDto(int Id, string Email, string FirstName, string LastName);
    private record VendorDto(int Id, string BusinessName);
    private record ProductDto(int Id, string Name, decimal Price);
    private record OrderDto(int Id, decimal TotalAmount, string Status);
    private record SubscriptionDto(int Id, string Status);
    private record PayoutDto(int Id, decimal Amount, string Status);

    #region Auth boundaries (spot-check)

    [Fact]
    public async Task AdminEndpoint_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/admin/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminEndpoint_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/admin/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminEndpoint_AsVendor_ReturnsForbidden()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/admin/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region GET /api/v1/admin/dashboard

    [Fact]
    public async Task GetDashboard_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/v1/admin/users

    [Fact]
    public async Task GetAllUsers_AsAdmin_ReturnsOkWithUsers()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<PagedResponse<UserDto>>(response);
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUserById_AsAdmin_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/users/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/v1/admin/vendors

    [Fact]
    public async Task GetAllVendors_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/vendors");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<PagedResponse<VendorDto>>(response);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingVendors_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/vendors/pending");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ApproveVendor_AsAdmin_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.PatchAsync("/api/v1/admin/vendors/999999/approve", null);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SuspendVendor_AsAdmin_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.PatchAsJsonAsync("/api/v1/admin/vendors/999999/suspend", new
        {
            reason = "Policy violation"
        });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    #endregion

    #region GET /api/v1/admin/products

    [Fact]
    public async Task GetAllProducts_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<PagedResponse<ProductDto>>(response);
        result.Should().NotBeNull();
    }

    #endregion

    #region GET /api/v1/admin/orders

    [Fact]
    public async Task GetAllOrders_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/orders");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<PagedResponse<OrderDto>>(response);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOrderById_AsAdmin_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/orders/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/v1/admin/subscriptions

    [Fact]
    public async Task GetAllSubscriptions_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/subscriptions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<PagedResponse<SubscriptionDto>>(response);
        result.Should().NotBeNull();
    }

    #endregion

    #region GET /api/v1/admin/payouts

    [Fact]
    public async Task GetAllPayouts_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/payouts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<PagedResponse<PayoutDto>>(response);
        result.Should().NotBeNull();
    }

    #endregion

    #region GET /api/v1/admin/reports

    [Fact]
    public async Task GetRevenueReport_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/reports/revenue");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetVendorPerformance_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/reports/vendors");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductAnalytics_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/reports/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCustomerAnalytics_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/admin/reports/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
