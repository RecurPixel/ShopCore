using ShopCore.Application.Common.Models;
using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedList<UserDto>>
{
    public Task<PaginatedList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
