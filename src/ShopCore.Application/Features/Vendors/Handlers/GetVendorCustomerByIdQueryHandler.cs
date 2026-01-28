using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerById;

public class GetVendorCustomerByIdQueryHandler : IRequestHandler<GetVendorCustomerByIdQuery, VendorCustomerDetailDto?>
{
    public Task<VendorCustomerDetailDto?> Handle(
        GetVendorCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}
