using ShopCore.Application.Common.Models;
using ShopCore.Application.CustomerInvitations.DTOs;

namespace ShopCore.Application.CustomerInvitations.Queries.GetMyCustomerInvitations;

public record GetMyCustomerInvitationsQuery(
    InvitationStatus? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<CustomerInvitationDto>>;
