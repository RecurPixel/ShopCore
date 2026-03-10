namespace ShopCore.Application.Common.Interfaces;

/// <summary>
/// Factory for creating and managing payment gateway instances
/// </summary>
public interface IPaymentGatewayFactory
{
    /// <summary>
    /// Gets a payment gateway by type
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when gateway is not enabled</exception>
    /// <exception cref="NotSupportedException">Thrown when gateway is not implemented</exception>
    IPaymentGateway GetGateway(PaymentGateway gateway);

    /// <summary>
    /// Gets all available/enabled gateways
    /// </summary>
    IReadOnlyCollection<IPaymentGateway> GetAvailableGateways();

    /// <summary>
    /// Checks if a gateway is enabled in configuration
    /// </summary>
    bool IsGatewayEnabled(PaymentGateway gateway);

    /// <summary>
    /// Gets the default gateway configured in settings
    /// </summary>
    IPaymentGateway GetDefaultGateway();
}
