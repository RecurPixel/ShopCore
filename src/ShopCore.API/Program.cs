using ShopCore.API.Extensions;
using ShopCore.API.Middleware;
using ShopCore.Application;
using ShopCore.Infrastructure;
using ShopCore.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// API Configuration
builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRateLimitingPolicies();

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
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerInDevelopment();
app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
