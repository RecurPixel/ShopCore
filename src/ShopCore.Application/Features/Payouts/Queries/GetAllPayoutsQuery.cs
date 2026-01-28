using ShopCore.Application.Common.Models;
using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetAllPayouts;

public record GetAllPayoutsQuery(
    string? Status = null,
    int? VendorId = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<PayoutDto>>;
