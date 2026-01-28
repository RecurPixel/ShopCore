using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetailDto?>
{
    public Task<UserDetailDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
