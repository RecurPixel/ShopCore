using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetCustomerAnalytics;

public record GetCustomerAnalyticsQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<CustomerAnalyticsReportDto>;
