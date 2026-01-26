using ShopCore.Application.Auth.Commands.ForgotPassword;
using ShopCore.Application.Auth.Commands.Login;
using ShopCore.Application.Auth.Commands.Logout;
using ShopCore.Application.Auth.Commands.RefreshToken;
using ShopCore.Application.Auth.Commands.RegisterUser;
using ShopCore.Application.Auth.Commands.VerifyEmail;
using ShopCore.Application.Auth.DTOs;
using ShopCore.Application.Users.DTOs;
using ShopCore.Application.Users.Queries.GetCurrentUser;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/v1/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(
        [FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(
        [FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Logout and invalidate refresh token
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand());
        return NoContent();
    }

    // POST /api/v1/auth/forgot-password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);

        // Always return 204 to avoid email enumeration
        return NoContent();
    }

    // POST /api/v1/auth/reset-password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // POST /api/v1/auth/verify-email
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // --------------------
    // Current user
    // --------------------

    // GET /api/v1/auth/me
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
    {
        var user = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(user);
    }
}
