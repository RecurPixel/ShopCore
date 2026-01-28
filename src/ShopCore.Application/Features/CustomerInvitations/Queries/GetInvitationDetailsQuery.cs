using ShopCore.Application.CustomerInvitations.DTOs;

namespace ShopCore.Application.CustomerInvitations.Queries.GetInvitationDetails;

public record GetInvitationDetailsQuery(string InvitationToken) : IRequest<InvitationDetailsDto>;
