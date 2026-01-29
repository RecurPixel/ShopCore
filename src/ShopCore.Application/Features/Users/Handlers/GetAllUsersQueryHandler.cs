using ShopCore.Application.Common.Models;
using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedList<UserDto>>
{
    public Task<PaginatedList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Fetch all users from database
        // 2. Apply filters if provided (role, status, search term)
        // 3. Apply pagination (request.Page, request.PageSize)
        // 4. Sort by creation date or name
        // 5. Map to UserDto list
        // 6. Return PaginatedList<UserDto>
        return Task.FromResult(new PaginatedList<UserDto>([], 0, request.Page, request.PageSize));
    }
}
