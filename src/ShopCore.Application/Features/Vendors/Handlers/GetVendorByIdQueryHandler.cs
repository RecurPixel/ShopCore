using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorById;

public class GetVendorByIdQueryHandler : IRequestHandler<GetVendorByIdQuery, VendorPublicProfileDto?>
{
    public Task<VendorPublicProfileDto?> Handle(
        GetVendorByIdQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}
