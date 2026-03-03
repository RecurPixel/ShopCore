using ShopCore.Application.Notifications.Commands.DeleteNotification;
using ShopCore.Application.Notifications.Commands.MarkAllNotificationsRead;
using ShopCore.Application.Notifications.Commands.MarkNotificationRead;
using ShopCore.Application.Notifications.DTOs;
using ShopCore.Application.Notifications.Queries.GetMyNotifications;
using ShopCore.Application.Notifications.Queries.GetUnreadCount;

namespace ShopCore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Returns paginated notifications for the current user.</summary>
    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetMyNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool unreadOnly = false)
    {
        var result = await _mediator.Send(new GetMyNotificationsQuery(page, pageSize, unreadOnly));
        return Ok(result);
    }

    /// <summary>Returns the count of unread notifications for the current user.</summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var count = await _mediator.Send(new GetUnreadCountQuery());
        return Ok(count);
    }

    /// <summary>Marks a single notification as read.</summary>
    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _mediator.Send(new MarkNotificationReadCommand(id));
        return NoContent();
    }

    /// <summary>Marks all notifications as read for the current user.</summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        await _mediator.Send(new MarkAllNotificationsReadCommand());
        return NoContent();
    }

    /// <summary>Soft-deletes a notification.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteNotificationCommand(id));
        return NoContent();
    }
}
