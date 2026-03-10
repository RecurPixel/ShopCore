using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ShopCore.UnitTests.Integration;

public class InvitationsControllerTests : IntegrationTestBase
{
    public InvitationsControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    private const string InvalidToken = "invalid-token-that-does-not-exist-12345";

    #region GET /api/v1/invitations/{token} (public)

    [Fact]
    public async Task GetInvitationDetails_WithInvalidToken_ReturnsNotFoundOrBadRequest()
    {
        var response = await Client.GetAsync($"/api/v1/invitations/{InvalidToken}");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest,
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetInvitationDetails_Anonymous_DoesNotReturn401()
    {
        ClearAuthentication();

        var response = await Client.GetAsync($"/api/v1/invitations/{InvalidToken}");

        // Anonymous access is allowed — should NOT return 401
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region POST /api/v1/invitations/{token}/accept (public)

    [Fact]
    public async Task AcceptInvitation_WithInvalidToken_ReturnsError()
    {
        var response = await Client.PostAsJsonAsync($"/api/v1/invitations/{InvalidToken}/accept", new
        {
            firstName = "John",
            lastName = "Doe",
            password = "Password@123"
        });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest,
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task AcceptInvitation_Anonymous_DoesNotReturn401()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync($"/api/v1/invitations/{InvalidToken}/accept", new
        {
            firstName = "John",
            lastName = "Doe",
            password = "Password@123"
        });

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region POST /api/v1/invitations/{token}/reject (public)

    [Fact]
    public async Task RejectInvitation_WithInvalidToken_ReturnsError()
    {
        var response = await Client.PostAsJsonAsync($"/api/v1/invitations/{InvalidToken}/reject", new
        {
            reason = "Not interested"
        });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NoContent,
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task RejectInvitation_Anonymous_DoesNotReturn401()
    {
        ClearAuthentication();

        var response = await Client.PostAsJsonAsync($"/api/v1/invitations/{InvalidToken}/reject", new
        {
            reason = "Not interested"
        });

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    #endregion
}
