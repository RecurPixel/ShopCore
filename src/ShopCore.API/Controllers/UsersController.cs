using ShopCore.Api.Files;
using ShopCore.Application.Users.Commands.ChangePassword;
using ShopCore.Application.Users.Commands.UpdateCurrentUser;
using ShopCore.Application.Users.Commands.UploadUserAvatar;
using ShopCore.Application.Users.DTOs;
using ShopCore.Application.Users.Queries.GetCurrentUser;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/v1/users/me
    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var user = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(user);
    }

    // PUT /api/v1/users/me
    [HttpPut]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(
        [FromBody] UpdateCurrentUserCommand command)
    {
        var user = await _mediator.Send(command);
        return Ok(user);
    }

    // POST /api/v1/users/me/avatar
    [HttpPost("avatar")]
    public async Task<ActionResult<string>> UploadAvatar(IFormFile file)
    {
        var command = new UploadUserAvatarCommand(
            new FormFileAdapter(file));

        var url = await _mediator.Send(command);
        return Ok(url);
    }

    // POST /api/v1/users/me/change-password
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}
