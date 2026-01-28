using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.ProcessRefund;

public record ProcessRefundCommand(
    int OrderId,
    List<int> OrderItemIds,
    string Reason
) : IRequest<RefundResultDto>;
