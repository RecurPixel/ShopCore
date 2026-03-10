using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RecurPixel.Notify;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Domain.Entities;
using ShopCore.Domain.Enums;
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

        // Database — provider selected via "Database:Provider" config key
        // Supported values: "SqlServer" (default), "Postgres"
        var dbProvider = configuration["Database:Provider"] ?? "SqlServer";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (dbProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
            {
                // Railway injects DATABASE_URL as a postgres:// URI; fall back to appsettings
                var connStr = Environment.GetEnvironmentVariable("DATABASE_URL")
                              ?? configuration.GetConnectionString("Postgres")
                              ?? throw new InvalidOperationException(
                                  "Postgres connection string not found. Set DATABASE_URL env var or ConnectionStrings:Postgres.");
                options.UseNpgsql(connStr);
            }
            else
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        });

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

            case FileStorageProvider.S3:
            case FileStorageProvider.Storj: // Storj DCS is S3-compatible — same implementation, different endpoint config
                services.AddScoped<IFileStorageService, S3FileStorageService>();
                break;

            default:
                services.AddScoped<IFileStorageService, LocalFileStorageService>();
                break;
        }

        services.AddInAppChannel(opts =>
        opts.UseHandler<IApplicationDbContext>(async (notification, db) =>
        {
            if (!int.TryParse(notification.UserId, out var userId))
                return new NotifyResult { Success = false, Error = $"Invalid userId in InApp notification: '{notification.UserId}'" };

            var typeStr = notification.Metadata.TryGetValue("type", out object? t) ? t?.ToString() : null;
            if (!Enum.TryParse<NotificationType>(typeStr, out var notifType))
                notifType = NotificationType.System;

            int? referenceId = null;
            if (notification.Metadata.TryGetValue("referenceId", out var rid) && rid is not null)
                referenceId = Convert.ToInt32(rid);

            var referenceType = notification.Metadata.TryGetValue("referenceType", out var rt) ? rt?.ToString() : null;

            await db.Notifications.AddAsync(new ShopCore.Domain.Entities.Notification
            {
                UserId = userId,
                Title = notification.Subject ?? string.Empty,
                Message = notification.Body,
                Type = notifType,
                ReferenceId = referenceId,
                ReferenceType = referenceType,
                IsRead = false
            });

            await db.SaveChangesAsync(CancellationToken.None);

            return new NotifyResult { Success = true };
        }));

        services.AddRecurPixelNotify(
        notifyOptions =>
        {
            configuration.GetSection("Notify").Bind(notifyOptions);

        },
        orchestratorOptions =>
        {
            // Auth events
            orchestratorOptions.DefineEvent("auth.welcome", e => e.UseChannels("email", "inapp", "telegram").WithFallback("telegram"));
            orchestratorOptions.DefineEvent("auth.verify-email", e => e.UseChannels("email"));
            orchestratorOptions.DefineEvent("auth.password-reset", e => e.UseChannels("email"));

            // Order events
            orchestratorOptions.DefineEvent("order.placed", e => e.UseChannels("email", "inapp"));
            orchestratorOptions.DefineEvent("order.cancelled", e => e.UseChannels("email", "inapp"));
            orchestratorOptions.DefineEvent("order.refund", e => e.UseChannels("email", "inapp"));

            // Subscription & delivery events
            orchestratorOptions.DefineEvent("subscription.created", e => e.UseChannels("email", "inapp"));
            orchestratorOptions.DefineEvent("delivery.skipped", e => e.UseChannels("inapp"));

            // Billing events
            orchestratorOptions.DefineEvent("invoice.paid", e => e.UseChannels("email", "inapp"));

            // Vendor events
            orchestratorOptions.DefineEvent("vendor.approved", e => e.UseChannels("email", "inapp"));
            orchestratorOptions.DefineEvent("vendor.suspended", e => e.UseChannels("email", "inapp"));
            orchestratorOptions.DefineEvent("vendor.payout", e => e.UseChannels("email", "inapp"));

            orchestratorOptions.OnDelivery<IApplicationDbContext>(async (result, db) =>
            {

                await db.NotificationLogs.AddAsync(new NotificationLog
                {
                    Channel = result.Channel,
                    Provider = result.Provider,
                    Recipient = result.Recipient ?? "Unknown",
                    Status = result.Success ? "Sent" : "Failed",
                    ProviderId = result.ProviderId,
                    Error = result.Error,
                    SentAt = result.SentAt
                });

                await db.SaveChangesAsync(CancellationToken.None);
            });
        });


        services.AddScoped<INotificationService, NotificationService>();

        // Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IPdfService, PdfService>();
        services.AddScoped<ITaxService, TaxService>();

        // Location Service with Google Maps API
        services.Configure<GoogleMapsSettings>(
            configuration.GetSection("GoogleMapsSettings"));
        services.AddHttpClient<ILocationService, LocationService>();

        // Payment Gateways
        services.Configure<PaymentGatewayOptions>(
            configuration.GetSection(PaymentGatewayOptions.SectionName));

        // Register HttpClient for Razorpay
        services.AddHttpClient("Razorpay");

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
