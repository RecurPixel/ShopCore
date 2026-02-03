using ShopCore.Application.Common.Models;
using ShopCore.Application.CustomerInvitations.DTOs;
using ShopCore.Application.CustomerInvitations.Queries.GetMyCustomerInvitations;

namespace ShopCore.Application.CustomerInvitations.Handlers;

public class GetMyCustomerInvitationsQueryHandler
    : IRequestHandler<GetMyCustomerInvitationsQuery, PaginatedList<CustomerInvitationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyCustomerInvitationsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<CustomerInvitationDto>> Handle(
        GetMyCustomerInvitationsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId == 0)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = _currentUserService.UserId;

        var vendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(VendorProfile), userId);

        var query = _context.CustomerInvitations
            .Include(ci => ci.Vendor)
            .Where(ci => ci.VendorId == vendor.Id);

        if (request.Status.HasValue)
        {
            query = query.Where(ci => ci.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(ci => ci.SentAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ci => new CustomerInvitationDto(
                ci.Id,
                ci.VendorId,
                ci.Vendor.BusinessName,
                ci.CustomerName,
                ci.PhoneNumber,
                ci.Email,
                ci.DeliveryAddress,
                ci.Pincode,
                ci.Status,
                ci.SentAt,
                ci.ExpiresAt,
                ci.AcceptedAt
            ))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PaginatedList<CustomerInvitationDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}
