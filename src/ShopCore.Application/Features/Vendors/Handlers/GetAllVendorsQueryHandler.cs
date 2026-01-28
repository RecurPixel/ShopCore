using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetAllVendors;

public class GetAllVendorsQueryHandler : IRequestHandler<GetAllVendorsQuery, PaginatedList<VendorProfileDto>>
{
    public Task<PaginatedList<VendorProfileDto>> Handle(GetAllVendorsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
