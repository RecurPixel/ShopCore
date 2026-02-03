using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.ConfirmPayment;

/// <summary>
/// Command to confirm a payment after Razorpay checkout completion.
/// Frontend sends the Razorpay callback data for server-side verification.
/// </summary>
public record ConfirmPaymentCommand(
    string RazorpayOrderId,      // razorpay_order_id from callback
    string RazorpayPaymentId,    // razorpay_payment_id from callback
    string RazorpaySignature     // razorpay_signature for verification
) : IRequest<PaymentConfirmationDto>;
