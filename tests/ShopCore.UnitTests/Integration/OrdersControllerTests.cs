using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;

namespace ShopCore.UnitTests.Integration;

public class OrdersControllerTests : IntegrationTestBase
{
    public OrdersControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region DTOs

    public record OrderDto
    {
        public int Id { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public decimal SubTotal { get; init; }
        public decimal Tax { get; init; }
        public decimal DeliveryFee { get; init; }
        public decimal Total { get; init; }
        public string Status { get; init; } = string.Empty;
        public string PaymentStatus { get; init; } = string.Empty;
        public List<OrderItemDto> Items { get; init; } = [];
    }

    public record OrderItemDto
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal Total { get; init; }
    }

    public record ProductDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int VendorId { get; init; }
    }

    public record AddressDto
    {
        public int Id { get; init; }
    }

    #endregion

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
        List<AddressDto>? addresses = await DeserializeAsync<List<AddressDto>>(addressesResponse);

        if (addresses?.Count > 0)
        {
            var orderRequest = new
            {
                addressId = addresses.First().Id,
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
