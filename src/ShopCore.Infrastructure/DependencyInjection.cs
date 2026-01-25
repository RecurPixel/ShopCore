using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ShopCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Database
        // services.AddDbContext<ApplicationDbContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        // );

        // services.AddScoped<IApplicationDbContext>(provider =>
        //     provider.GetRequiredService<ApplicationDbContext>()
        // );

        // // Services
        // services.AddScoped<ICurrentUserService, CurrentUserService>();
        // services.AddTransient<IDateTime, DateTimeService>();
        // services.AddTransient<IEmailService, EmailService>();

        // // Authentication
        // services.AddScoped<IJwtTokenService, JwtTokenService>();
        // services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
