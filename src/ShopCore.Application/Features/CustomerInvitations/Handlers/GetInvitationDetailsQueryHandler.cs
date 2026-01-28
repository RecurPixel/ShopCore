using System.Text.Json;
using ShopCore.Application.CustomerInvitations.DTOs;
using ShopCore.Application.CustomerInvitations.Queries.GetInvitationDetails;

namespace ShopCore.Application.CustomerInvitations.Handlers;

public class GetInvitationDetailsQueryHandler
    : IRequestHandler<GetInvitationDetailsQuery, InvitationDetailsDto>
{
    private readonly IApplicationDbContext _context;

    public GetInvitationDetailsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InvitationDetailsDto> Handle(
        GetInvitationDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var invitation = await _context.CustomerInvitations
            .Include(ci => ci.Vendor)
            .FirstOrDefaultAsync(ci => ci.InvitationToken == request.InvitationToken, cancellationToken)
            ?? throw new NotFoundException(nameof(CustomerInvitation), request.InvitationToken);

        var subscriptionItems = JsonSerializer.Deserialize<List<InvitationSubscriptionItem>>(
            invitation.SubscriptionItemsJson) ?? new List<InvitationSubscriptionItem>();

        var productIds = subscriptionItems.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var items = subscriptionItems.Select(si =>
        {
            var product = products.FirstOrDefault(p => p.Id == si.ProductId);
            return new InvitationItemDto(
                si.ProductId,
                product?.Name ?? "Unknown Product",
                product?.Images.FirstOrDefault()?.ImageUrl,
                si.Quantity,
                si.UnitPrice,
                GetFrequencyDescription(invitation.Frequency)
            );
        }).ToList();

        var dailyTotal = items.Sum(i => i.Quantity * i.UnitPrice);
        var monthlyEstimate = CalculateMonthlyEstimate(dailyTotal, invitation.Frequency);

        return new InvitationDetailsDto(
            invitation.Vendor.BusinessName,
            invitation.Vendor.BusinessLogo,
            items,
            invitation.Frequency,
            invitation.PreferredDeliveryTime,
            monthlyEstimate,
            invitation.DepositAmount
        );
    }

    private static string GetFrequencyDescription(SubscriptionFrequency frequency)
    {
        return frequency switch
        {
            SubscriptionFrequency.Daily => "Daily",
            SubscriptionFrequency.EveryTwoDays => "Every 2 days",
            SubscriptionFrequency.Weekly => "Weekly",
            SubscriptionFrequency.BiWeekly => "Bi-weekly",
            SubscriptionFrequency.Monthly => "Monthly",
            _ => "Custom"
        };
    }

    private static decimal CalculateMonthlyEstimate(decimal dailyTotal, SubscriptionFrequency frequency)
    {
        return frequency switch
        {
            SubscriptionFrequency.Daily => dailyTotal * 30,
            SubscriptionFrequency.EveryTwoDays => dailyTotal * 15,
            SubscriptionFrequency.Weekly => dailyTotal * 4,
            SubscriptionFrequency.BiWeekly => dailyTotal * 2,
            SubscriptionFrequency.Monthly => dailyTotal,
            _ => dailyTotal * 30
        };
    }
}
