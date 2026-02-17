using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Infrastructure.Data;
using ShopCore.Infrastructure.FileStorage;
using ShopCore.Infrastructure.Identity;
using ShopCore.Infrastructure.PaymentGateways;
using ShopCore.Infrastructure.PaymentGateways.Providers;
using ShopCore.Infrastructure.Services;

namespace ShopCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // HttpContextAccessor (required for CurrentUserService)
        services.AddHttpContextAccessor();

        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>()
        );

        // File Storage
        services.Configure<FileStorageOptions>(
            configuration.GetSection(FileStorageOptions.SectionName));

        // Register file storage service based on configuration
        var fileStorageOptions = configuration
            .GetSection(FileStorageOptions.SectionName)
            .Get<FileStorageOptions>() ?? new FileStorageOptions();

        switch (fileStorageOptions.Provider)
        {
            case FileStorageProvider.Local:
                services.AddScoped<IFileStorageService, LocalFileStorageService>();
                break;

            case FileStorageProvider.AzureBlob:
                services.AddScoped<IFileStorageService, AzureBlobFileStorageService>();
                break;

            default:
                // Default to local storage
                services.AddScoped<IFileStorageService, LocalFileStorageService>();
                break;
        }

        // Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IPdfService, PdfService>();
        services.AddScoped<ITaxService, TaxService>();
        services.AddScoped<ILocationService, LocationService>();

        // Payment Gateways
        services.Configure<PaymentGatewayOptions>(
            configuration.GetSection(PaymentGatewayOptions.SectionName));

        // Register all gateway implementations
        services.AddScoped<IPaymentGateway, RazorpayGateway>();
        services.AddScoped<IPaymentGateway, StripeGateway>();
        services.AddScoped<IPaymentGateway, CashOnDeliveryGateway>();

        // Factory for resolving gateways
        services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();


        // Authentication
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
