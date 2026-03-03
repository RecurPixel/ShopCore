using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecurPixel.Notify.Core.Models;
using RecurPixel.Notify.Core.Options;
using RecurPixel.Notify.Orchestrator.Options;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Domain.Entities;
using ShopCore.Domain.Enums;

namespace ShopCore.Infrastructure.Services;

/// <summary>
/// Wires the SDK's OnDelivery hook and InApp handler at startup so every send attempt
/// (success or failure) is persisted to the NotificationLog table, and every InApp
/// delivery is stored in the Notifications table.
/// </summary>
internal sealed class NotificationDeliveryLogger : IHostedService
{
    private readonly OrchestratorOptions _opts;
    private readonly InAppOptions _inAppOpts;
    private readonly IServiceScopeFactory _scopeFactory;

    public NotificationDeliveryLogger(
        OrchestratorOptions opts,
        InAppOptions inAppOpts,
        IServiceScopeFactory scopeFactory)
    {
        _opts = opts;
        _inAppOpts = inAppOpts;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken ct)
    {
        // Wire the InApp handler — called by InAppChannel.SendAsync on every inapp delivery
        _inAppOpts.Handler = async (payload, token) =>
        {
            if (!int.TryParse(payload.To, out var userId))
                return new NotifyResult { Success = false, Error = $"Invalid userId in InApp payload: '{payload.To}'" };

            var typeStr = payload.Metadata.TryGetValue("type", out object? t) ? t?.ToString() : null;
            if (!Enum.TryParse<NotificationType>(typeStr, out var notifType))
                notifType = NotificationType.System;

            int? referenceId = null;
            if (payload.Metadata.TryGetValue("referenceId", out var rid) && rid is not null)
                referenceId = Convert.ToInt32(rid);

            var referenceType = payload.Metadata.TryGetValue("referenceType", out var rt) ? rt?.ToString() : null;

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            await db.Notifications.AddAsync(new Notification
            {
                UserId = userId,
                Title = payload.Subject ?? string.Empty,
                Message = payload.Body,
                Type = notifType,
                ReferenceId = referenceId,
                ReferenceType = referenceType,
                IsRead = false
            }, token);

            await db.SaveChangesAsync(token);

            return new NotifyResult { Success = true };
        };

        // Wire the delivery audit hook — called after every channel send attempt
        _opts.OnDelivery(async result =>
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

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

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
