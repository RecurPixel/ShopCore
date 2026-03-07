using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class AuthControllerTests : IntegrationTestBase
{
    public AuthControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginRequest = new
        {
            email = "customer@test.com",
            password = "Customer@123"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        AuthResponse? result = await DeserializeAsync<AuthResponse>(response);
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("customer@test.com");
        result.User.Role.Should().Be("Customer");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            email = "customer@test.com",
            password = "WrongPassword123"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            email = "nonexistent@test.com",
            password = "Password123"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithEmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new
        {
            email = "",
            password = "Password123"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithInvalidEmailFormat_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new
        {
            email = "not-an-email",
            password = "Password123"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Registration Tests

    [Fact]
    public async Task Register_WithValidData_ReturnsCreated()
    {
        // Arrange
        var registerRequest = new
        {
            Email = $"newuser_{Guid.NewGuid():N}@test.com",
            Password = "NewUser@123",
            // ConfirmPassword = "NewUser@123",
            FirstName = "New",
            LastName = "User",
            PhoneNumber = "9876543210"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsConflict()
    {
        // Arrange
        var registerRequest = new
        {
            Email = "customer@test.com", // Already exists
            Password = "NewUser@123",
            // ConfirmPassword = "NewUser@123",
            FirstName = "New",
            LastName = "User",
            PhoneNumber = "9876543210"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Verify the error message indicates email is already registered
        var content = await response.Content.ReadAsStringAsync();
        content.Should().ContainAny("Email already registered", "already exists", "already registered");
    }

    [Fact]
    public async Task Register_WithMismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            Email = $"newuser_{Guid.NewGuid():N}@test.com",
            Password = "NewUser@123",
            ConfirmPassword = "DifferentPassword@123",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            email = $"newuser_{Guid.NewGuid():N}@test.com",
            password = "weak", // Too short/simple
            confirmPassword = "weak",
            firstName = "New",
            lastName = "User"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Token Refresh Tests

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
    {
        // Arrange - First login to get tokens
        HttpResponseMessage loginResponse = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "customer@test.com",
            password = "Customer@123"
        });

        AuthResponse? authResult = await DeserializeAsync<AuthResponse>(loginResponse);
        authResult.Should().NotBeNull();

        // Act - Refresh the token
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/refresh-token", new
        {
            accessToken = authResult!.AccessToken,
            refreshToken = authResult.RefreshToken
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        AuthResponse? refreshResult = await DeserializeAsync<AuthResponse>(response);
        refreshResult.Should().NotBeNull();
        refreshResult!.AccessToken.Should().NotBeNullOrEmpty();
        refreshResult.AccessToken.Should().NotBe(authResult.AccessToken);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshRequest = new
        {
            AccessToken = "invalid.access.token",
            RefreshToken = "invalid-refresh-token"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/auth/refresh-token", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Logout Tests

    [Fact]
    public async Task Logout_WhenAuthenticated_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Logout_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Protected Endpoint Access Tests

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
}
