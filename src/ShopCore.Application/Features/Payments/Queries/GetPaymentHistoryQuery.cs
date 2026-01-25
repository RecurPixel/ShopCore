using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Queries.GetPaymentHistory;

public record GetPaymentHistoryQuery : IRequest<List<PaymentHistoryDto>>;
