using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ShopCore.Infrastructure.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ShopCore.UnitTests.Infrastructure;

public abstract class IntegrationTestBase : IClassFixture<ShopCoreWebApplicationFactory>, IAsyncDisposable
{
    protected readonly HttpClient Client;
    protected readonly ShopCoreWebApplicationFactory Factory;
    private readonly IServiceScope _scope;
    protected readonly ApplicationDbContext DbContext;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected IntegrationTestBase(ShopCoreWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        factory.SeedDatabase();
        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    /// <summary>
    /// Authenticate as a customer user
    /// </summary>
    protected async Task AuthenticateAsCustomerAsync()
    {
        await AuthenticateAsync("customer@test.com", "Customer@123");
    }

    /// <summary>
    /// Authenticate as a vendor user
    /// </summary>
    protected async Task AuthenticateAsVendorAsync()
    {
        await AuthenticateAsync("vendor@test.com", "Vendor@123");
    }

    /// <summary>
    /// Authenticate as an admin user
    /// </summary>
    protected async Task AuthenticateAsAdminAsync()
    {
        await AuthenticateAsync("admin@shopcore.com", "Admin@123");
    }

    /// <summary>
    /// Authenticate with specific credentials
    /// </summary>
    protected async Task AuthenticateAsync(string email, string password)
    {
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email,
            password
        });

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);

        string? token = doc.RootElement.GetProperty("accessToken").GetString();
        token.Should().NotBeNullOrEmpty();

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Clear authentication
    /// </summary>
    protected void ClearAuthentication()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Deserialize response content
    /// </summary>
    protected static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        string content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    /// <summary>
    /// Assert response is success and deserialize
    /// </summary>
    protected static async Task<T> AssertSuccessAndDeserializeAsync<T>(HttpResponseMessage response)
    {
        response.IsSuccessStatusCode.Should().BeTrue(
            $"Expected success but got {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

        T? result = await DeserializeAsync<T>(response);
        result.Should().NotBeNull();

        return result!;
    }

    public async ValueTask DisposeAsync()
    {
        _scope.Dispose();
        Client.Dispose();
        await Task.CompletedTask;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Generic response wrapper for paginated results
/// </summary>
public record PagedResponse<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalItems { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

/// <summary>
/// Auth response DTO
/// </summary>
public record AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserInfo User { get; init; } = null!;
}

public record UserInfo
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
