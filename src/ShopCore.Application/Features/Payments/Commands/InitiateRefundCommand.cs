using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.InitiateRefund;

public record InitiateRefundCommand(
    int OrderId,
    decimal Amount,
    string? Reason = null
) : IRequest<RefundDto>;
