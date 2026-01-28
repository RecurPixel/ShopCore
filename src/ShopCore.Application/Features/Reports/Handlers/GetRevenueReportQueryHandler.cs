using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetRevenueReport;

public class GetRevenueReportQueryHandler : IRequestHandler<GetRevenueReportQuery, RevenueReportDto>
{
    public Task<RevenueReportDto> Handle(GetRevenueReportQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
