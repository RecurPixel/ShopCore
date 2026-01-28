using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<UserDetailDto?>;
