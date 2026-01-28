using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetRevenueReport;

public record GetRevenueReportQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string Period = "month"
) : IRequest<RevenueReportDto>;
