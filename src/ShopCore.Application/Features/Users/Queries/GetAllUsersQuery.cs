using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(
    string? Search = null,
    string? Role = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<UserDto>>;
