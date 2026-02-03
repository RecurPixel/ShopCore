namespace ShopCore.Application.Payments.Commands.RecordCODPayment;

/// <summary>
/// Records that a Cash on Delivery payment was collected.
/// Typically called by vendor or delivery personnel after collecting cash.
/// </summary>
public record RecordCODPaymentCommand(int OrderId) : IRequest;
