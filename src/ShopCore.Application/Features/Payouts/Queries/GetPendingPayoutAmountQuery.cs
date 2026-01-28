using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetPendingPayoutAmount;

public record GetPendingPayoutAmountQuery : IRequest<PendingPayoutDto>;
