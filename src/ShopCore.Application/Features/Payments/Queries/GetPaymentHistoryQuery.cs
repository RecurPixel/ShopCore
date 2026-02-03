using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Queries.GetPaymentHistory;

public record GetPaymentHistoryQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<PaymentHistoryDto>>;
