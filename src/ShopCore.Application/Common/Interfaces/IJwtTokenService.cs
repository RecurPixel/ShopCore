using System.Security.Claims;
using ShopCore.Domain.Entities;

namespace ShopCore.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}
