using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Queries.GetNearbyVendors;

public class GetNearbyVendorsQueryHandler : IRequestHandler<GetNearbyVendorsQuery, List<NearbyVendorDto>>
{
    public Task<List<NearbyVendorDto>> Handle(GetNearbyVendorsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
