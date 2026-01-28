using ShopCore.Application.Auth.Commands.ForgotPassword;
using ShopCore.Application.Auth.Commands.Login;
using ShopCore.Application.Auth.Commands.Logout;
using ShopCore.Application.Auth.Commands.RefreshToken;
using ShopCore.Application.Auth.Commands.RegisterUser;
using ShopCore.Application.Auth.Commands.ResendVerification;
using ShopCore.Application.Auth.Commands.VerifyEmail;
using ShopCore.Application.Auth.DTOs;

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
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(
        [FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Register), result);
    }

    // POST /api/v1/auth/login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // POST /api/v1/auth/refresh-token
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(
        [FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // POST /api/v1/auth/logout
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand());
        return NoContent();
    }

    // POST /api/v1/auth/forgot-password
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        // Always return 204 to avoid email enumeration
        return NoContent();
    }

    // POST /api/v1/auth/reset-password
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // POST /api/v1/auth/verify-email
    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // POST /api/v1/auth/resend-verification
    [AllowAnonymous]
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(
        [FromBody] ResendVerificationCommand command)
    {
        await _mediator.Send(command);
        // Always return 204 to avoid email enumeration
        return NoContent();
    }
}
