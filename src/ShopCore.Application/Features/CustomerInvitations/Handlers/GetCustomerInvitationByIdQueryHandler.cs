using ShopCore.Application.CustomerInvitations.DTOs;

namespace ShopCore.Application.CustomerInvitations.Queries.GetCustomerInvitationById;

public class GetCustomerInvitationByIdQueryHandler : IRequestHandler<GetCustomerInvitationByIdQuery, CustomerInvitationDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCustomerInvitationByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CustomerInvitationDto?> Handle(
        GetCustomerInvitationByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.CustomerInvitations
            .AsNoTracking()
            .Include(ci => ci.Vendor)
                .ThenInclude(v => v.User)
            .Include(ci => ci.InvitedUser)
            .Where(ci => ci.Id == request.Id && ci.VendorId == _currentUser.VendorId)
            .Select(ci => new CustomerInvitationDto
            {
                Id = ci.Id,
                VendorId = ci.VendorId,
                VendorName = ci.Vendor.BusinessName,
                CustomerName = ci.CustomerName,
                PhoneNumber = ci.PhoneNumber,
                Email = ci.Email,
                DeliveryAddress = ci.DeliveryAddress,
                InvitationToken = ci.InvitationToken,
                Status = ci.Status.ToString(),
                InvitedUserId = ci.InvitedUserId ?? 0,
                AcceptedAt = ci.AcceptedAt,
                RejectedAt = ci.RejectedAt,
                ExpiresAt = ci.ExpiresAt,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}