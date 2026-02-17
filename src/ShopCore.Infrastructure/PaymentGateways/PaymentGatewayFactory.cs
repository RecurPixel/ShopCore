using Microsoft.Extensions.Options;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Domain.Enums;

namespace ShopCore.Infrastructure.PaymentGateways;

/// <summary>
/// Factory for creating and managing payment gateway instances
/// </summary>
public class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly PaymentGatewayOptions _options;
    private readonly IReadOnlyDictionary<PaymentGateway, IPaymentGateway> _gateways;

    public PaymentGatewayFactory(
        IOptions<PaymentGatewayOptions> options,
        IEnumerable<IPaymentGateway> gateways)
    {
        _options = options.Value;
        _gateways = gateways.ToDictionary(g => g.GatewayType);
    }

    public IPaymentGateway GetGateway(PaymentGateway gateway)
    {
        // Manual gateway (COD) is controlled by EnableCOD setting
        if (gateway == PaymentGateway.Manual)
        {
            if (!_options.EnableCOD)
                throw new InvalidOperationException("Cash on Delivery is not enabled");
        }
        else if (!_options.EnabledGateways.Contains(gateway))
        {
            throw new InvalidOperationException($"Gateway '{gateway}' is not enabled");
        }

        if (!_gateways.TryGetValue(gateway, out var impl))
            throw new NotSupportedException($"Gateway '{gateway}' is not implemented");

        return impl;
    }

    public IReadOnlyCollection<IPaymentGateway> GetAvailableGateways()
    {
        var available = _options.EnabledGateways
            .Where(g => _gateways.ContainsKey(g))
            .Select(g => _gateways[g])
            .ToList();

        // Add COD if enabled and implemented
        if (_options.EnableCOD && _gateways.TryGetValue(PaymentGateway.Manual, out var codGateway))
        {
            available.Add(codGateway);
        }

        return available;
    }

    public bool IsGatewayEnabled(PaymentGateway gateway)
    {
        if (gateway == PaymentGateway.Manual)
            return _options.EnableCOD;

        return _options.EnabledGateways.Contains(gateway);
    }

    public IPaymentGateway GetDefaultGateway()
    {
        return GetGateway(_options.DefaultGateway);
    }
}
