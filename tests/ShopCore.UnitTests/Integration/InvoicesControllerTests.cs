using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;

namespace ShopCore.UnitTests.Integration;

public class InvoicesControllerTests : IntegrationTestBase
{
    public InvoicesControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region GET /api/v1/invoices/{id} (auth required)

    [Fact]
    public async Task GetInvoiceById_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/invoices/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetInvoiceById_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/invoices/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetInvoiceById_AsVendor_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsVendorAsync();

        var response = await Client.GetAsync("/api/v1/invoices/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetInvoiceById_AsAdmin_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/invoices/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/v1/invoices/{id}/download (auth required)

    [Fact]
    public async Task DownloadInvoice_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/invoices/1/download");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DownloadInvoice_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/invoices/999999/download");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
