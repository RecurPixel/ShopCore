namespace ShopCore.Application.Addresses.Queries.GetMyAddresses;

public class GetMyAddressesQueryHandler : IRequestHandler<GetMyAddressesQuery, object>
{
    public Task<object> Handle(GetMyAddressesQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
