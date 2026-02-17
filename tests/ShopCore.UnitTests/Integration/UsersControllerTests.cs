using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;

namespace ShopCore.UnitTests.Integration;

public class UsersControllerTests : IntegrationTestBase
{
    public UsersControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    #region DTOs

    public record UserProfileDto
    {
        public int Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public string Role { get; init; } = string.Empty;
    }

    public record AddressDto
    {
        public int Id { get; init; }
        public string AddressLine1 { get; init; } = string.Empty;
        public string? AddressLine2 { get; init; }
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public bool IsDefault { get; init; }
    }

    #endregion

    #region Profile Tests

    [Fact]
    public async Task GetProfile_WhenAuthenticated_ReturnsProfile()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/users/me/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        UserProfileDto? profile = await DeserializeAsync<UserProfileDto>(response);
        profile.Should().NotBeNull();
        profile!.Email.Should().Be("customer@test.com");
        profile.FirstName.Should().Be("Test");
        profile.LastName.Should().Be("Customer");
        profile.Role.Should().Be("Customer");
    }

    [Fact]
    public async Task GetProfile_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/users/me/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateProfile_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        var updateRequest = new
        {
            firstName = "Updated",
            lastName = "Customer",
            phone = "9876543210"
        };

        // Act
        HttpResponseMessage response = await Client.PutAsJsonAsync("/api/v1/users/me/profile", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify the update
        HttpResponseMessage profileResponse = await Client.GetAsync("/api/v1/users/me/profile");
        UserProfileDto? profile = await DeserializeAsync<UserProfileDto>(profileResponse);
        profile!.FirstName.Should().Be("Updated");
    }

    #endregion

    #region Address Tests

    [Fact]
    public async Task GetAddresses_WhenAuthenticated_ReturnsAddresses()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/users/me/addresses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddAddress_WithValidData_ReturnsCreated()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        var addressRequest = new
        {
            addressLine1 = "456 New Street",
            city = "New City",
            state = "New State",
            postalCode = "654321",
            country = "India",
            isDefault = false
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/users/me/addresses", addressRequest);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddAddress_WithMissingFields_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        var addressRequest = new
        {
            addressLine1 = "", // Required field is empty
            city = "New City"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/users/me/addresses", addressRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteAddress_WithValidId_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // First create an address
        await Client.PostAsJsonAsync("/api/v1/users/me/addresses", new
        {
            addressLine1 = "Delete Me Street",
            city = "Delete City",
            state = "Delete State",
            postalCode = "111111",
            country = "India",
            isDefault = false
        });

        // Get addresses
        HttpResponseMessage addressesResponse = await Client.GetAsync("/api/v1/users/me/addresses");
        List<AddressDto>? addresses = await DeserializeAsync<List<AddressDto>>(addressesResponse);

        AddressDto? addressToDelete = addresses?.FirstOrDefault(a => a.AddressLine1 == "Delete Me Street");

        if (addressToDelete != null)
        {
            // Act
            HttpResponseMessage response = await Client.DeleteAsync($"/api/v1/users/me/addresses/{addressToDelete.Id}");

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
        }
    }

    #endregion

    #region Change Password Tests

    [Fact]
    public async Task ChangePassword_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        var changePasswordRequest = new
        {
            currentPassword = "Customer@123",
            newPassword = "NewCustomer@123",
            confirmNewPassword = "NewCustomer@123"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/users/me/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Change it back for other tests
        await Client.PostAsJsonAsync("/api/v1/users/me/change-password", new
        {
            currentPassword = "NewCustomer@123",
            newPassword = "Customer@123",
            confirmNewPassword = "Customer@123"
        });
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        var changePasswordRequest = new
        {
            currentPassword = "WrongPassword@123",
            newPassword = "NewPassword@123",
            confirmNewPassword = "NewPassword@123"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/users/me/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_WithMismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        var changePasswordRequest = new
        {
            currentPassword = "Customer@123",
            newPassword = "NewPassword@123",
            confirmNewPassword = "DifferentPassword@123"
        };

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/users/me/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Wishlist Tests

    [Fact]
    public async Task GetWishlist_WhenAuthenticated_ReturnsWishlist()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Act
        HttpResponseMessage response = await Client.GetAsync("/api/v1/users/me/wishlist");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddToWishlist_WithValidProduct_ReturnsSuccess()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        // Get a product
        HttpResponseMessage productsResponse = await Client.GetAsync("/api/v1/products");
        PagedResponse<ProductsControllerTests.ProductDto>? products =
            await DeserializeAsync<PagedResponse<ProductsControllerTests.ProductDto>>(productsResponse);
        int productId = products!.Items.First().Id;

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/users/me/wishlist", new
        {
            productId
        });

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.NoContent);
    }

    #endregion
}
