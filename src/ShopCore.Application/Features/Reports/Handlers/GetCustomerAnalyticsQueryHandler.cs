using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetCustomerAnalytics;

public class GetCustomerAnalyticsQueryHandler : IRequestHandler<GetCustomerAnalyticsQuery, CustomerAnalyticsReportDto>
{
    public Task<CustomerAnalyticsReportDto> Handle(GetCustomerAnalyticsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
