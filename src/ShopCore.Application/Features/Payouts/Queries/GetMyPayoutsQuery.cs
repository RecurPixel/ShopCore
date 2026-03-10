using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetMyPayouts;

public record GetMyPayoutsQuery(
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<PayoutDto>>;
