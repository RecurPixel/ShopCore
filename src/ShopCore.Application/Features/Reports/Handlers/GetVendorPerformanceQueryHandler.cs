using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetVendorPerformance;

public class GetVendorPerformanceQueryHandler : IRequestHandler<GetVendorPerformanceQuery, VendorPerformanceReportDto>
{
    public Task<VendorPerformanceReportDto> Handle(GetVendorPerformanceQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
