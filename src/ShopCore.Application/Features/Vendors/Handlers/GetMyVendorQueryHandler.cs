using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendor;

public class GetMyVendorQueryHandler : IRequestHandler<GetMyVendorQuery, VendorProfileDto>
{
    public Task<VendorProfileDto> Handle(
        GetMyVendorQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new VendorProfileDto());
    }
}
