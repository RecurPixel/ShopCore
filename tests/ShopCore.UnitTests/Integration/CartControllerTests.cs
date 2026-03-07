using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;

namespace ShopCore.UnitTests.Integration;

public class CartControllerTests : IntegrationTestBase
{
    public CartControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region DTOs

    public record CartDto
    {
        public int Id { get; init; }
        public List<CartItemDto> Items { get; init; } = [];
        public decimal SubTotal { get; init; }
        public decimal Tax { get; init; }
        public decimal Discount { get; init; }
        public decimal Total { get; init; }
        public string? CouponCode { get; init; }
    }

    public record CartItemDto
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int Quantity { get; init; }
        public decimal Total { get; init; }
    }

    public record ProductDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
    }

    #endregion

    #region Get Cart Tests

    [Fact]
    public async Task GetCart_WhenAuthenticated_ReturnsCart()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/cart");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        CartDto? cart = await DeserializeAsync<CartDto>(response);
        cart.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCart_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/cart");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Add to Cart Tests

    [Fact]
    public async Task AddToCart_WithValidProduct_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Get a product ID
        HttpResponseMessage productsResponse = await Client.GetAsync("/api/v1/products");
        PagedResponse<ProductDto>? products = await DeserializeAsync<PagedResponse<ProductDto>>(productsResponse);
        int productId = products!.Items.First().Id;

        var addRequest = new
        {
            productId,
            quantity = 2
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/cart/items", addRequest);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddToCart_WithInvalidProduct_ReturnsNotFound()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        var addRequest = new
        {
            productId = 999999,
            quantity = 1
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/cart/items", addRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddToCart_WithZeroQuantity_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        HttpResponseMessage productsResponse = await Client.GetAsync("/api/v1/products");
        PagedResponse<ProductDto>? products = await DeserializeAsync<PagedResponse<ProductDto>>(productsResponse);
        int productId = products!.Items.First().Id;

        var addRequest = new
        {
            productId,
            quantity = 0
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/cart/items", addRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update Cart Item Tests

    [Fact]
    public async Task UpdateCartItem_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // First add an item to cart
        HttpResponseMessage productsResponse = await Client.GetAsync("/api/v1/products");
        PagedResponse<ProductDto>? products = await DeserializeAsync<PagedResponse<ProductDto>>(productsResponse);
        int productId = products!.Items.First().Id;

        await Client.PostAsJsonAsync("/api/v1/cart/items", new { productId, quantity = 1 });

        // Get cart to find the item ID
        HttpResponseMessage cartResponse = await Client.GetAsync("/api/v1/cart");
        CartDto? cart = await DeserializeAsync<CartDto>(cartResponse);

        if (cart?.Items.Count > 0)
        {
            int itemId = cart.Items.First().Id;

            // Act
            HttpResponseMessage response = await Client.PutAsJsonAsync($"/api/v1/cart/items/{itemId}", new
            {
                quantity = 3
            });

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        }
    }

    #endregion

    #region Remove from Cart Tests

    [Fact]
    public async Task RemoveFromCart_WithValidItem_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // First add an item to cart
        HttpResponseMessage productsResponse = await Client.GetAsync("/api/v1/products");
        PagedResponse<ProductDto>? products = await DeserializeAsync<PagedResponse<ProductDto>>(productsResponse);
        int productId = products!.Items.First().Id;

        await Client.PostAsJsonAsync("/api/v1/cart/items", new { productId, quantity = 1 });

        // Get cart to find the item ID
        HttpResponseMessage cartResponse = await Client.GetAsync("/api/v1/cart");
        CartDto? cart = await DeserializeAsync<CartDto>(cartResponse);

        if (cart?.Items.Count > 0)
        {
            int itemId = cart.Items.First().Id;

            // Act
            HttpResponseMessage response = await Client.DeleteAsync($"/api/v1/cart/items/{itemId}");

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        }
    }

    #endregion

    #region Clear Cart Tests

    [Fact]
    public async Task ClearCart_RemovesAllItems()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Add an item first
        HttpResponseMessage productsResponse = await Client.GetAsync("/api/v1/products");
        PagedResponse<ProductDto>? products = await DeserializeAsync<PagedResponse<ProductDto>>(productsResponse);
        int productId = products!.Items.First().Id;

        await Client.PostAsJsonAsync("/api/v1/cart/items", new { productId, quantity = 1 });

        // Act
        HttpResponseMessage response = await Client.DeleteAsync("/api/v1/cart/clear");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        // Verify cart is empty
        HttpResponseMessage cartResponse = await Client.GetAsync("/api/v1/cart");
        CartDto? cart = await DeserializeAsync<CartDto>(cartResponse);
        cart!.Items.Should().BeEmpty();
    }

    #endregion

    #region Validate Cart Tests

    [Fact]
    public async Task ValidateCart_ReturnsValidationResult()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Add an item so the cart is not empty
        HttpResponseMessage productsResponse = await Client.GetAsync("/api/v1/products");
        PagedResponse<ProductDto>? products = await DeserializeAsync<PagedResponse<ProductDto>>(productsResponse);
        int productId = products!.Items.First().Id;
        await Client.PostAsJsonAsync("/api/v1/cart/items", new { productId, quantity = 1 });

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/cart/validate", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
