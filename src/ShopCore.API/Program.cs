using Serilog;
using ShopCore.API.Extensions;
using ShopCore.API.Middleware;
using ShopCore.Application;
using ShopCore.Infrastructure;
using ShopCore.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// PORT binding — Railway (and most container platforms) inject $PORT at runtime
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Serilog — reads full config from appsettings.json "Serilog" section.
// Skipped in Testing: the two-stage ReloadableLogger bootstrap can only be
// frozen once, which conflicts with WebApplicationFactory creating multiple hosts.
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Host.UseSerilog((ctx, _, config) =>
        config.ReadFrom.Configuration(ctx.Configuration)
              .Enrich.FromLogContext());
}

// API Configuration
builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRateLimitingPolicies();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Application & Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ApplicationDbContextSeeder>();

var app = builder.Build();

// Database initialization
await app.ApplyMigrationsAsync();

// Middleware pipeline
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });
}
app.UseSwaggerInDevelopment();
app.UseHttpsRedirection();
app.UseCors();
if (!app.Environment.IsEnvironment("Testing"))
    app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
