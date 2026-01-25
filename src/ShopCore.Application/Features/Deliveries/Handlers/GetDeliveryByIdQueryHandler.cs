namespace ShopCore.Application.Deliveries.Queries.GetDeliveryById;

public class GetDeliveryByIdQueryHandler : IRequestHandler<GetDeliveryByIdQuery, object>
{
    public Task<object> Handle(GetDeliveryByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
