using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetVendorPerformance;

public record GetVendorPerformanceQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Top = 10
) : IRequest<VendorPerformanceReportDto>;
