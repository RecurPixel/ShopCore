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

    /// <summary>
    /// Registers a new the requested resource.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>RegisterResponse</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(
        [FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Register), result);
    }

    /// <summary>
    /// Authenticates user and returns the requested resource.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>LoginResponse</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Refreshes token.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>RefreshTokenResponse</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(
        [FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Logs out the current the requested resource.
    /// </summary>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand());
        return NoContent();
    }

    /// <summary>
    /// Initiates password recovery for password.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        // Always return 204 to avoid email enumeration
        return NoContent();
    }

    /// <summary>
    /// Resets password.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Verifies email.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Resends verification.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
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
