using ShopCore.Application.Common.Exceptions;
using System.Text.Json;

namespace ShopCore.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var problemDetails = exception switch
        {
            ValidationException validationEx => new ValidationProblemDetails(validationEx.Errors)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = validationEx.Message,
                Instance = context.Request.Path
            },

            NotFoundException notFoundEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Status = StatusCodes.Status404NotFound,
                Detail = notFoundEx.Message,
                Instance = context.Request.Path
            },

            UnauthorizedException unauthorizedEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = unauthorizedEx.Message,
                Instance = context.Request.Path
            },

            ForbiddenException forbiddenEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = forbiddenEx.Message,
                Instance = context.Request.Path
            },

            ConflictException conflictEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = "A conflict occurred.",
                Status = StatusCodes.Status409Conflict,
                Detail = conflictEx.Message,
                Instance = context.Request.Path
            },

            BadRequestException badRequestEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = badRequestEx.Message,
                Instance = context.Request.Path
            },

            _ => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An error occurred while processing your request.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = _environment.IsDevelopment() ? exception.Message : "An internal error occurred.",
                Instance = context.Request.Path
            }
        };

        // Add TraceId for debugging
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        // Add stack trace in development
        if (_environment.IsDevelopment() && exception is not ValidationException)
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }
}