using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class CategoriesControllerTests : IntegrationTestBase
{
    public CategoriesControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    private record CategoryDto(int Id, string Name, string Slug, string? Description, int DisplayOrder);
    private record ProductDto(int Id, string Name, decimal Price);

    #region GET /api/v1/categories

    [Fact]
    public async Task GetCategories_ReturnsOkWithList()
    {
        var response = await Client.GetAsync("/api/v1/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<PagedResponse<CategoryDto>>(response);
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    #endregion

    #region GET /api/v1/categories/{id}

    [Fact]
    public async Task GetCategory_WithValidId_ReturnsOk()
    {
        var all = await Client.GetAsync("/api/v1/categories");
        var categories = await DeserializeAsync<PagedResponse<CategoryDto>>(all);
        int id = categories!.Items.First().Id;

        var response = await Client.GetAsync($"/api/v1/categories/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<CategoryDto>(response);
        result!.Id.Should().Be(id);
        result.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetCategory_WithInvalidId_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/v1/categories/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/v1/categories/{id}/products

    [Fact]
    public async Task GetCategoryProducts_WithValidId_ReturnsOk()
    {
        var all = await Client.GetAsync("/api/v1/categories");
        var categories = await DeserializeAsync<PagedResponse<CategoryDto>>(all);
        int id = categories!.Items.First().Id;

        var response = await Client.GetAsync($"/api/v1/categories/{id}/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region POST /api/v1/categories (Admin only)

    [Fact]
    public async Task CreateCategory_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync("/api/v1/categories", new
        {
            name = "New Category",
            slug = "new-category",
            displayOrder = 10
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCategory_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/categories", new
        {
            name = "New Category",
            slug = "new-category",
            displayOrder = 10
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateCategory_AsAdmin_ReturnsCreated()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/categories", new
        {
            name = "Test New Category",
            slug = "test-new-category",
            description = "A test category",
            displayOrder = 99
        });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }

    #endregion

    #region PUT /api/v1/categories/{id} (Admin only)

    [Fact]
    public async Task UpdateCategory_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();
        var all = await Client.GetAsync("/api/v1/categories");
        var categories = await DeserializeAsync<PagedResponse<CategoryDto>>(all);
        int id = categories!.Items.First().Id;

        var response = await Client.PutAsJsonAsync($"/api/v1/categories/{id}", new
        {
            id,
            name = "Updated Category Name",
            slug = "updated-category-name",
            displayOrder = 1
        });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateCategory_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PutAsJsonAsync("/api/v1/categories/1", new
        {
            id = 1,
            name = "Hacked",
            slug = "hacked",
            displayOrder = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region DELETE /api/v1/categories/{id} (Admin only)

    [Fact]
    public async Task DeleteCategory_AsAdmin_ReturnsNoContent()
    {
        await AuthenticateAsAdminAsync();

        // Create a category first so we can safely delete it
        var createResponse = await Client.PostAsJsonAsync("/api/v1/categories", new
        {
            name = "To Be Deleted",
            slug = "to-be-deleted",
            displayOrder = 100
        });
        var created = await DeserializeAsync<CategoryDto>(createResponse);

        var response = await Client.DeleteAsync($"/api/v1/categories/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCategory_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.DeleteAsync("/api/v1/categories/1");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion
}
