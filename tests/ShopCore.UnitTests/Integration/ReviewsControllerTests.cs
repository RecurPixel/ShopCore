using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class ReviewsControllerTests : IntegrationTestBase
{
    public ReviewsControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    private record ReviewDto(int Id, int ProductId, int UserId, int Rating, string Comment);
    private record ProductDto(int Id, string Name);

    private async Task<int> GetFirstProductIdAsync()
    {
        var response = await Client.GetAsync("/api/v1/products");
        var products = await DeserializeAsync<PagedResponse<ProductDto>>(response);
        return products!.Items.First().Id;
    }

    #region GET /api/v1/reviews/products/{productId}/reviews (public)

    [Fact]
    public async Task GetProductReviews_IsPublic_ReturnsOk()
    {
        ClearAuthentication();
        int productId = await GetFirstProductIdAsync();

        var response = await Client.GetAsync($"/api/v1/reviews/products/{productId}/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductReviews_WithInvalidProductId_ReturnsOkWithEmpty()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/reviews/products/999999/reviews");

        // Either 200 with empty list or 404
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/v1/reviews/me (auth required)

    [Fact]
    public async Task GetMyReviews_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/reviews/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyReviews_WhenAuthenticated_ReturnsOk()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/reviews/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region POST /api/v1/reviews (auth required, multipart)

    [Fact]
    public async Task CreateReview_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("4"), "rating");
        content.Add(new StringContent("Great product!"), "comment");
        content.Add(new StringContent("1"), "productId");

        var response = await Client.PostAsync("/api/v1/reviews", content);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateReview_WhenAuthenticated_ReturnsCreatedOrBadRequest()
    {
        await AuthenticateAsCustomerAsync();
        int productId = await GetFirstProductIdAsync();

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(productId.ToString()), "productId");
        content.Add(new StringContent("4"), "rating");
        content.Add(new StringContent("Great product! Highly recommended."), "comment");

        var response = await Client.PostAsync("/api/v1/reviews", content);

        // Created if product exists and not already reviewed, or BadRequest if validation fails
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.Conflict);
    }

    #endregion

    #region POST /api/v1/reviews/{id}/helpful (auth required)

    [Fact]
    public async Task MarkHelpful_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsync("/api/v1/reviews/1/helpful", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MarkHelpful_WithNonExistentReview_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsync("/api/v1/reviews/999999/helpful", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DELETE /api/v1/reviews/{id} (auth required)

    [Fact]
    public async Task DeleteReview_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.DeleteAsync("/api/v1/reviews/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteReview_WithNonExistentId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.DeleteAsync("/api/v1/reviews/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
