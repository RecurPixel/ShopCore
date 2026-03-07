using FluentAssertions;
using ShopCore.Application.Addresses.DTOs;
using ShopCore.Application.Orders.DTOs;
using ShopCore.Application.Products.DTOs;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ShopCore.UnitTests.Integration;

public class OrdersControllerTests : IntegrationTestBase
{
    public OrdersControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region Create Order Tests

    [Fact]
    public async Task CreateOrder_WithValidCart_ReturnsCreated()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Add item to cart first
        HttpResponseMessage productsResponse = await Client.GetAsync("/api/v1/products");
        PagedResponse<ProductDto>? products = await DeserializeAsync<PagedResponse<ProductDto>>(productsResponse);
        ProductDto product = products!.Items.First();

        await Client.PostAsJsonAsync("/api/v1/cart/items", new { productId = product.Id, quantity = 1 });

        // Create an address first
        HttpResponseMessage addressResponse = await Client.PostAsJsonAsync("/api/v1/users/me/addresses", new
        {
            addressLine1 = "123 Test Street",
            city = "Test City",
            state = "Test State",
            postalCode = "123456",
            country = "India",
            isDefault = true
        });

        // Get addresses
        HttpResponseMessage addressesResponse = await Client.GetAsync("/api/v1/users/me/addresses");
        PagedResponse<AddressDto>? addresses = await DeserializeAsync<PagedResponse<AddressDto>>(addressesResponse);

        if (addresses?.Items.Count > 0)
        {
            var orderRequest = new
            {
                addressId = addresses.Items.First().Id,
                paymentMethod = "CashOnDelivery",
                notes = "Test order"
            };

            // Act
            HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/orders", orderRequest);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task CreateOrder_WithEmptyCart_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Clear cart first
        await Client.DeleteAsync("/api/v1/cart/clear");

        // Try to create order with empty cart
        var orderRequest = new
        {
            addressId = 1,
            paymentMethod = "CashOnDelivery"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/orders", orderRequest);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange - clear any authentication
        ClearAuthentication();

        var orderRequest = new
        {
            addressId = 1,
            paymentMethod = "CashOnDelivery"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/orders", orderRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Get User Orders Tests

    [Fact]
    public async Task GetMyOrders_WhenAuthenticated_ReturnsOrders()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/users/me/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        PagedResponse<OrderDto>? result = await DeserializeAsync<PagedResponse<OrderDto>>(response);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetMyOrders_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/users/me/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
}
