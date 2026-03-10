using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomers;

public record GetVendorCustomersQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<VendorCustomerDto>>;
