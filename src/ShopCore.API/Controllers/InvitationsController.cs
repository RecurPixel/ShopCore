using ShopCore.Application.CustomerInvitations.Commands.AcceptInvitation;
using ShopCore.Application.CustomerInvitations.Commands.RejectInvitation;
using ShopCore.Application.CustomerInvitations.DTOs;
using ShopCore.Application.CustomerInvitations.Queries.GetInvitationDetails;

namespace ShopCore.Api.Controllers;

/// <summary>
/// Public endpoints for customer invitation acceptance/rejection
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class InvitationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvitationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/v1/invitations/{token}
    [AllowAnonymous]
    [HttpGet("{token}")]
    public async Task<ActionResult<InvitationDetailsDto>> GetInvitationDetails(string token)
    {
        var details = await _mediator.Send(new GetInvitationDetailsQuery(token));
        return Ok(details);
    }

    // POST /api/v1/invitations/{token}/accept
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

    // POST /api/v1/invitations/{token}/reject
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
