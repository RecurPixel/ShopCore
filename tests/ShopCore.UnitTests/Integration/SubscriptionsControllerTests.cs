using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class SubscriptionsControllerTests : IntegrationTestBase
{
    public SubscriptionsControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region POST /api/v1/subscriptions (auth required)

    [Fact]
    public async Task CreateSubscription_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync("/api/v1/subscriptions", new
        {
            vendorId = 1,
            productId = 1,
            quantity = 1,
            frequency = "Weekly"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateSubscription_WithInvalidData_ReturnsError()
    {
        await AuthenticateAsCustomerAsync();

        // Invalid product id → 404 or 400
        var response = await Client.PostAsJsonAsync("/api/v1/subscriptions", new
        {
            vendorId = 999999,
            productId = 999999,
            quantity = 1,
            frequency = 1
        });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound,
            HttpStatusCode.UnprocessableEntity);
    }

    #endregion

    #region POST /api/v1/subscriptions/{id}/pause (auth required)

    [Fact]
    public async Task PauseSubscription_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsync("/api/v1/subscriptions/1/pause", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PauseSubscription_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsync("/api/v1/subscriptions/999999/pause", null);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    #endregion

    #region POST /api/v1/subscriptions/{id}/resume (auth required)

    [Fact]
    public async Task ResumeSubscription_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsync("/api/v1/subscriptions/1/resume", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ResumeSubscription_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsync("/api/v1/subscriptions/999999/resume", null);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    #endregion

    #region DELETE /api/v1/subscriptions/{id} (auth required)

    [Fact]
    public async Task CancelSubscription_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.DeleteAsync("/api/v1/subscriptions/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CancelSubscription_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.DeleteAsync("/api/v1/subscriptions/999999");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    #endregion

    #region POST /api/v1/subscriptions/deliveries/{deliveryId}/skip (auth required)

    [Fact]
    public async Task SkipDelivery_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync("/api/v1/subscriptions/deliveries/1/skip", new { reason = "Not home" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SkipDelivery_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/subscriptions/deliveries/999999/skip", new { reason = "Not home" });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    #endregion

    #region POST /api/v1/subscriptions/invoices/{invoiceId}/pay (auth required)

    [Fact]
    public async Task PayInvoice_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync("/api/v1/subscriptions/invoices/1/pay", new
        {
            paymentMethod = "Cash",
            paymentTransactionId = "txn-123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PayInvoice_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/subscriptions/invoices/999999/pay", new
        {
            paymentMethod = "Cash",
            paymentTransactionId = "txn-123"
        });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    #endregion
}
