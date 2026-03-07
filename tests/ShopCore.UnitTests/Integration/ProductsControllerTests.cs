using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Text.Json;

namespace ShopCore.UnitTests.Integration;

public class ProductsControllerTests : IntegrationTestBase
{
    public ProductsControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region DTOs for Products

    public record ProductDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Unit { get; init; } = string.Empty;
        public int StockQuantity { get; init; }
        public string Status { get; init; } = string.Empty;
        public bool IsFeatured { get; init; }
        public int CategoryId { get; init; }
        public string CategoryName { get; init; } = string.Empty;
        public int VendorId { get; init; }
        public string VendorName { get; init; } = string.Empty;
    }

    #endregion

    #region List Products Tests

    [Fact]
    public async Task GetProducts_ReturnsPagedList()
    {
        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        PagedResponse<ProductDto>? result = await DeserializeAsync<PagedResponse<ProductDto>>(response);
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.TotalItems.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetProducts_WithPagination_ReturnsCorrectPage()
    {
        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/products?page=1&pageSize=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        PagedResponse<ProductDto>? result = await DeserializeAsync<PagedResponse<ProductDto>>(response);
        result.Should().NotBeNull();
        result!.Items.Count.Should().BeLessThanOrEqualTo(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
    }

    #endregion

    #region Get Product by ID Tests

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Arrange - First get a product ID from the list
        HttpResponseMessage listResponse = await Client.GetAsync("/api/v1/products");
        PagedResponse<ProductDto>? products = await DeserializeAsync<PagedResponse<ProductDto>>(listResponse);
        int productId = products!.Items.First().Id;

        // Act
        HttpResponseMessage response = await Client.GetAsync($"/api/v1/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ProductDto? result = await DeserializeAsync<ProductDto>(response);
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().NotBeNullOrEmpty();
        result.Price.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ReturnsNotFound()
    {
        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/products/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Featured Products Tests

    [Fact]
    public async Task GetFeaturedProducts_ReturnsFeaturedOnly()
    {
        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/products/featured");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        PagedResponse<ProductDto>? result = await DeserializeAsync<PagedResponse<ProductDto>>(response);
        result.Should().NotBeNull();

        // All returned products should be featured
        result!.Items.Should().OnlyContain(p => p.IsFeatured);
    }

    #endregion

    #region Search Products Tests

    [Fact]
    public async Task SearchProducts_WithQuery_ReturnsMatchingProducts()
    {
        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/products/search?Search=apple");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        PagedResponse<ProductDto>? result = await DeserializeAsync<PagedResponse<ProductDto>>(response);
        result.Should().NotBeNull();

        // Should find the test apple product
        result!.Items.Should().Contain(p => p.Name.Contains("Apple", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchProducts_WithNoMatches_ReturnsEmptyList()
    {
        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/products/search?Search=nonexistentproduct123");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        PagedResponse<ProductDto>? result = await DeserializeAsync<PagedResponse<ProductDto>>(response);
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
    }

    #endregion

    #region Product Reviews Tests

    [Fact]
    public async Task GetProductReviews_ReturnsReviews()
    {
        // Arrange
        HttpResponseMessage listResponse = await Client.GetAsync("/api/v1/products");
        PagedResponse<ProductDto>? products = await DeserializeAsync<PagedResponse<ProductDto>>(listResponse);
        int productId = products!.Items.First().Id;

        // Act
        HttpResponseMessage response = await Client.GetAsync($"/api/v1/products/{productId}/reviews");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
