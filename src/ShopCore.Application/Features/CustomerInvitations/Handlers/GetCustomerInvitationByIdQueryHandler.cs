using ShopCore.Application.CustomerInvitations.DTOs;

namespace ShopCore.Application.CustomerInvitations.Queries.GetCustomerInvitationById;

public class GetCustomerInvitationByIdQueryHandler : IRequestHandler<GetCustomerInvitationByIdQuery, CustomerInvitationDto?>
{
    public Task<CustomerInvitationDto?> Handle(
        GetCustomerInvitationByIdQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}
