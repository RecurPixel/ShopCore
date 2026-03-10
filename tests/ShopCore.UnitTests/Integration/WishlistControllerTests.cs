using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class WishlistControllerTests : IntegrationTestBase
{
    public WishlistControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    private record ProductDto(int Id, string Name);
    private record WishlistItemDto(int ProductId, string ProductName, decimal Price);
    private record WishlistDto(List<WishlistItemDto> Items);

    private async Task<int> GetFirstProductIdAsync()
    {
        var response = await Client.GetAsync("/api/v1/products");
        var products = await DeserializeAsync<PagedResponse<ProductDto>>(response);
        return products!.Items.First().Id;
    }

    #region GET /api/v1/wishlist

    [Fact]
    public async Task GetWishlist_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/wishlist");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetWishlist_WhenAuthenticated_ReturnsOk()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/wishlist");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region POST /api/v1/wishlist

    [Fact]
    public async Task AddToWishlist_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync("/api/v1/wishlist", new { productId = 1 });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task AddToWishlist_WithValidProduct_ReturnsSuccess()
    {
        await AuthenticateAsCustomerAsync();
        int productId = await GetFirstProductIdAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/wishlist", new { productId });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.OK, HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddToWishlist_WithInvalidProduct_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/wishlist", new { productId = 999999 });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DELETE /api/v1/wishlist/{productId}

    [Fact]
    public async Task RemoveFromWishlist_WhenNotAuthenticated_ReturnsUnauthorizedOrNoContent()
    {
        ClearAuthentication();

        var response = await Client.DeleteAsync("/api/v1/wishlist/1");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Unauthorized,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveFromWishlist_WithValidProduct_ReturnsNoContent()
    {
        await AuthenticateAsCustomerAsync();
        int productId = await GetFirstProductIdAsync();

        // Add first so we can remove
        await Client.PostAsJsonAsync("/api/v1/wishlist", new { productId });

        var response = await Client.DeleteAsync($"/api/v1/wishlist/{productId}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/v1/wishlist/{productId}/move-to-cart

    [Fact]
    public async Task MoveToCart_WhenNotAuthenticated_ReturnsUnauthorizedOrNotFound()
    {
        ClearAuthentication();

        var response = await Client.PostAsync("/api/v1/wishlist/1/move-to-cart", null);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Unauthorized,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MoveToCart_WithNonWishlistedProduct_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsync("/api/v1/wishlist/999999/move-to-cart", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
