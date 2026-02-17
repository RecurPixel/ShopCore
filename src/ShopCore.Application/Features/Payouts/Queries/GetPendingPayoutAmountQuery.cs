using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetPendingPayoutAmount;

public record GetPendingPayoutAmountQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<PendingPayoutDto>;
