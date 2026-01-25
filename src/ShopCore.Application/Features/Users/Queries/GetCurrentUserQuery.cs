using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserProfileDto>;
