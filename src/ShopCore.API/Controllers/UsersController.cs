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
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var user = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(user);
    }

    // PUT /api/v1/users/me
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateCurrentUserCommand command)
    {
        var user = await _mediator.Send(command);
        return Ok(user);
    }

    // POST /api/v1/users/me/avatar
    [HttpPost("me/avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] UploadUserAvatarCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // POST /api/v1/users/me/change-password
    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}
