using ShopCore.Application.CustomerInvitations.Commands.AcceptInvitation;
using ShopCore.Application.CustomerInvitations.Commands.RejectInvitation;
using ShopCore.Application.CustomerInvitations.DTOs;
using ShopCore.Application.CustomerInvitations.Queries.GetInvitationDetails;

namespace ShopCore.Api.Controllers;

/// <summary>
/// Public endpoints for customer invitation acceptance/rejection
/// </summary>
[ApiController]
[Route("api/v1/invitations")]
public class InvitationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvitationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves invitation details.
    /// </summary>
    /// <param name="token">The token</param>
    /// <returns>InvitationDetailsDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpGet("{token}")]
    public async Task<ActionResult<InvitationDetailsDto>> GetInvitationDetails(string token)
    {
        var details = await _mediator.Send(new GetInvitationDetailsQuery(token));
        return Ok(details);
    }

    /// <summary>
    /// Creates or processes accept invitation.
    /// </summary>
    /// <param name="token">The token</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>InvitationAcceptedDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpPost("{token}/accept")]
    public async Task<ActionResult<InvitationAcceptedDto>> AcceptInvitation(
        string token,
        [FromBody] AcceptInvitationCommand command)
    {
        var finalCommand = command with { InvitationToken = token };
        var result = await _mediator.Send(finalCommand);
        return Ok(result);
    }

    /// <summary>
    /// Rejects invitation.
    /// </summary>
    /// <param name="token">The token</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpPost("{token}/reject")]
    public async Task<IActionResult> RejectInvitation(
        string token,
        [FromBody] RejectInvitationCommand command)
    {
        var finalCommand = command with { InvitationToken = token };
        await _mediator.Send(finalCommand);
        return NoContent();
    }
}
