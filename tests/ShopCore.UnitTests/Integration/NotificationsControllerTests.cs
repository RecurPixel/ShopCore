using FluentAssertions;
using ShopCore.UnitTests.Infrastructure;
using System.Net;

namespace ShopCore.UnitTests.Integration;

public class NotificationsControllerTests : IntegrationTestBase
{
    public NotificationsControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    private record NotificationDto(int Id, string Title, string Message, bool IsRead, string Type);

    #region GET /api/v1/notifications

    [Fact]
    public async Task GetNotifications_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/notifications");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetNotifications_WhenAuthenticated_ReturnsOk()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/notifications");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await DeserializeAsync<List<NotificationDto>>(response);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetNotifications_WithUnreadFilter_ReturnsOk()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/notifications?isRead=false");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/v1/notifications/unread-count

    [Fact]
    public async Task GetUnreadCount_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/v1/notifications/unread-count");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUnreadCount_WhenAuthenticated_ReturnsOk()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.GetAsync("/api/v1/notifications/unread-count");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        string content = await response.Content.ReadAsStringAsync();
        int.TryParse(content, out int count).Should().BeTrue("response should be an integer");
        count.Should().BeGreaterThanOrEqualTo(0);
    }

    #endregion

    #region PUT /api/v1/notifications/read-all

    [Fact]
    public async Task MarkAllAsRead_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PutAsync("/api/v1/notifications/read-all", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MarkAllAsRead_WhenAuthenticated_ReturnsNoContent()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PutAsync("/api/v1/notifications/read-all", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    #endregion

    #region PUT /api/v1/notifications/{id}/read

    [Fact]
    public async Task MarkAsRead_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.PutAsync("/api/v1/notifications/1/read", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MarkAsRead_WithNonExistentId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.PutAsync("/api/v1/notifications/999999/read", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DELETE /api/v1/notifications/{id}

    [Fact]
    public async Task DeleteNotification_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        ClearAuthentication();

        var response = await Client.DeleteAsync("/api/v1/notifications/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteNotification_WithNonExistentId_ReturnsNotFound()
    {
        await AuthenticateAsCustomerAsync();

        var response = await Client.DeleteAsync("/api/v1/notifications/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
