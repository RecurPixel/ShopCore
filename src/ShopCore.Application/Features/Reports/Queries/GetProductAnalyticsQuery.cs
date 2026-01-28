using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetProductAnalytics;

public record GetProductAnalyticsQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Top = 10
) : IRequest<ProductAnalyticsReportDto>;
