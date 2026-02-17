using ShopCore.Domain.Enums;

namespace ShopCore.Application.Payments.DTOs;

/// <summary>
/// Represents an available payment option/gateway
/// </summary>
public record PaymentOptionDto
{
    public PaymentGateway Gateway { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyCollection<PaymentMethod> SupportedMethods { get; init; } = Array.Empty<PaymentMethod>();
    public bool IsDefault { get; init; }
}
