using ShopCore.Domain.Enums;

namespace ShopCore.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string Email { get; }
    UserRole Role { get; }
    int? VendorId { get; }
    bool IsAuthenticated { get; }
}
