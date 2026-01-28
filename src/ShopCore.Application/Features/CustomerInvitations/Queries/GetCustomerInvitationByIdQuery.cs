using ShopCore.Application.CustomerInvitations.DTOs;

namespace ShopCore.Application.CustomerInvitations.Queries.GetCustomerInvitationById;

public record GetCustomerInvitationByIdQuery(int Id) : IRequest<CustomerInvitationDto?>;
