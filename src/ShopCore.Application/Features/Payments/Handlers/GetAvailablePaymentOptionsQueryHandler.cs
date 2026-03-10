using ShopCore.Application.Payments.DTOs;
using ShopCore.Application.Payments.Queries.GetAvailablePaymentOptions;

namespace ShopCore.Application.Payments.Handlers;

public class GetAvailablePaymentOptionsQueryHandler
    : IRequestHandler<GetAvailablePaymentOptionsQuery, IReadOnlyCollection<PaymentOptionDto>>
{
    private readonly IPaymentGatewayFactory _gatewayFactory;

    public GetAvailablePaymentOptionsQueryHandler(IPaymentGatewayFactory gatewayFactory)
    {
        _gatewayFactory = gatewayFactory;
    }

    public Task<IReadOnlyCollection<PaymentOptionDto>> Handle(
        GetAvailablePaymentOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var defaultGateway = _gatewayFactory.GetDefaultGateway();
        var gateways = _gatewayFactory.GetAvailableGateways();

        var options = gateways
            .Select(g => new PaymentOptionDto
            {
                Gateway = g.GatewayType,
                DisplayName = g.DisplayName,
                Description = g.Description,
                SupportedMethods = g.SupportedMethods,
                IsDefault = g.GatewayType == defaultGateway.GatewayType
            })
            .ToList();

        return Task.FromResult<IReadOnlyCollection<PaymentOptionDto>>(options);
    }
}
