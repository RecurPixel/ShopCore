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
        // 1. Fetch all vendors with status 'pending approval'
        // 2. Include vendor documents and submission details
        // 3. Include registration date and last updated date
        // 4. Sort by submission date (oldest first for review)
        // 5. Include basic vendor info (name, email, location)
        // 6. Include application completeness score
        // 7. Optionally apply pagination if large dataset
        // 8. Map to VendorProfileDto list and return
        return Task.FromResult(new List<VendorProfileDto>());
    }
}
