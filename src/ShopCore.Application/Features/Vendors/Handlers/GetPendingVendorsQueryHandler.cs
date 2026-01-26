using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetPendingVendors;

public class GetPendingVendorsQueryHandler
    : IRequestHandler<GetPendingVendorsQuery, List<VendorProfileDto>>
{
    public Task<List<VendorProfileDto>> Handle(
        GetPendingVendorsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new List<VendorProfileDto>());
    }
}
