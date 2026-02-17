using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Queries.GetAvailablePaymentOptions;

/// <summary>
/// Query to get all available/enabled payment options
/// </summary>
public record GetAvailablePaymentOptionsQuery : IRequest<IReadOnlyCollection<PaymentOptionDto>>;
