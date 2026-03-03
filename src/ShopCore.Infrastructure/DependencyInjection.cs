using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RecurPixel.Notify.Core.Channels;
using RecurPixel.Notify.Core.Extensions;
using RecurPixel.Notify.Core.Options;
using RecurPixel.Notify.Email.Smtp;
using RecurPixel.Notify.InApp;
using RecurPixel.Notify.Orchestrator.Extensions;
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

        // Notifications
        var notifyOptions = configuration.GetSection("Notify").Get<NotifyOptions>() ?? new NotifyOptions();
        services.AddRecurPixelNotify(notifyOptions);

        // ChannelDispatcher resolves IOptions<NotifyOptions> — bridge the singleton so it
        // receives the same configured instance (not an empty default from the options framework)
        services.AddSingleton<IOptions<NotifyOptions>>(Options.Create(notifyOptions));

        // Register channel adapters based on configuration
        if (notifyOptions.Email?.Provider == "smtp" && notifyOptions.Email.Smtp is not null)
            services.AddSmtpChannel(notifyOptions.Email.Smtp);

        // InApp channel — handler is wired at startup by NotificationDeliveryLogger.
        // AddInAppChannel() in the NuGet package registers the keyed service as "inapp:inapp"
        // but ChannelDispatcher resolves simple channels by bare channel name ("inapp"), so
        // we register manually with the correct key.
        var inAppOptions = new InAppOptions();
        services.AddSingleton(Options.Create(inAppOptions));
        services.AddKeyedSingleton<INotificationChannel, InAppChannel>("inapp");
        services.AddSingleton(inAppOptions);

        services.AddRecurPixelNotifyOrchestrator(o =>
        {
            // Auth events
            o.DefineEvent("auth.welcome",        e => e.UseChannels("email", "inapp"));
            o.DefineEvent("auth.verify-email",   e => e.UseChannels("email"));
            o.DefineEvent("auth.password-reset", e => e.UseChannels("email"));

            // Order events
            o.DefineEvent("order.placed",        e => e.UseChannels("email", "inapp"));
            o.DefineEvent("order.cancelled",     e => e.UseChannels("email", "inapp"));
            o.DefineEvent("order.refund",        e => e.UseChannels("email", "inapp"));

            // Subscription & delivery events
            o.DefineEvent("subscription.created", e => e.UseChannels("email", "inapp"));
            o.DefineEvent("delivery.skipped",      e => e.UseChannels("inapp"));

            // Billing events
            o.DefineEvent("invoice.paid", e => e.UseChannels("email", "inapp"));

            // Vendor events
            o.DefineEvent("vendor.approved",  e => e.UseChannels("email", "inapp"));
            o.DefineEvent("vendor.suspended", e => e.UseChannels("email", "inapp"));
            o.DefineEvent("vendor.payout",    e => e.UseChannels("email", "inapp"));
        });

        services.AddScoped<INotificationService, NotificationService>();
        services.AddHostedService<NotificationDeliveryLogger>();

        // Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IPdfService, PdfService>();
        services.AddScoped<ITaxService, TaxService>();
        services.AddScoped<ILocationService, LocationService>();

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
