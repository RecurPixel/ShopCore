namespace ShopCore.Application.Payments.DTOs;

public record PaymentIntentDto
{
    public string PaymentIntentId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "INR";
    public PaymentStatus Status { get; init; }
    public string? RazorpayOrderId { get; init; }
}
