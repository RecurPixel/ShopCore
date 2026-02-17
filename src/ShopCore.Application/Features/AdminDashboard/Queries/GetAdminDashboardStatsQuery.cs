using ShopCore.Application.AdminDashboard.DTOs;

namespace ShopCore.Application.AdminDashboard.Queries.GetAdminDashboardStats;

public record GetAdminDashboardStatsQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<AdminDashboardStatsDto>;
