using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CalculatePendingPayouts;

public record CalculatePendingPayoutsCommand : IRequest<PendingPayoutSummaryDto>;
