using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Queries.GetVendorsByPincode;

public class GetVendorsByPincodeQueryHandler : IRequestHandler<GetVendorsByPincodeQuery, List<NearbyVendorDto>>
{
    public Task<List<NearbyVendorDto>> Handle(GetVendorsByPincodeQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
