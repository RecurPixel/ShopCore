using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetProductAnalytics;

public class GetProductAnalyticsQueryHandler : IRequestHandler<GetProductAnalyticsQuery, ProductAnalyticsReportDto>
{
    public Task<ProductAnalyticsReportDto> Handle(GetProductAnalyticsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
