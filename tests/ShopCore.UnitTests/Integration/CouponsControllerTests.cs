using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class CouponsControllerTests : IntegrationTestBase
{
    public CouponsControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    private record CouponDto(int Id, string Code, string DiscountType, decimal DiscountValue, bool IsActive);
    private record CouponValidationResultDto(bool IsValid, string? Message, decimal? DiscountAmount);

    #region GET /api/v1/coupons/active (public)

    [Fact]
    public async Task GetActiveCoupons_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/v1/coupons/active");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<PagedResponse<CouponDto>>(response);
        result.Should().NotBeNull();
    }

    #endregion

    #region POST /api/v1/coupons/validate (auth required)

    [Fact]
    public async Task ValidateCoupon_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync("/api/v1/coupons/validate", new
        {
            code = "TESTCODE",
            orderAmount = 500m
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidateCoupon_WithInvalidCode_ReturnsResult()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/coupons/validate", new
        {
            code = "INVALIDCODE999",
            orderAmount = 500m
        });

        // Should return 200 with IsValid=false, or 404/400
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest);
    }

    #endregion

    #region GET /api/v1/coupons (Admin only)

    [Fact]
    public async Task GetAllCoupons_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/coupons");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllCoupons_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/coupons");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllCoupons_AsAdmin_ReturnsOk()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("/api/v1/coupons");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<PagedResponse<CouponDto>>(response);
        result.Should().NotBeNull();
    }

    #endregion

    #region POST /api/v1/coupons (Admin only)

    [Fact]
    public async Task CreateCoupon_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync("/api/v1/coupons", new
        {
            code = "SAVE10",
            discountType = "Percentage",
            discountValue = 10m,
            minimumOrderAmount = 100m,
            expiresAt = DateTime.UtcNow.AddDays(30)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCoupon_AsAdmin_ReturnsCreated()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/coupons", new
        {
            code = $"TEST{Guid.NewGuid():N}"[..12].ToUpper(),
            type = 1, // CouponType.Percentage
            discountPercentage = 10m,
            validFrom = DateTime.UtcNow,
            validUntil = DateTime.UtcNow.AddDays(30)
        });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }

    #endregion

    #region PATCH /api/v1/coupons/{id}/deactivate (Admin only)

    [Fact]
    public async Task DeactivateCoupon_AsCustomer_ReturnsForbidden()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PatchAsync("/api/v1/coupons/1/deactivate", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeactivateCoupon_WithInvalidId_AsAdmin_ReturnsNotFound()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.PatchAsync("/api/v1/coupons/999999/deactivate", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
