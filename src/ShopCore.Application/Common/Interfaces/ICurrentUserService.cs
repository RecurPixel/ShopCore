namespace ShopCore.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }

    /// <summary>
    /// Gets the UserId, throwing UnauthorizedAccessException if not authenticated.
    /// Use this in handlers that rely on controller-level [Authorize] attributes.
    /// </summary>
    int RequiredUserId => UserId ?? throw new UnauthorizedAccessException("User not authenticated");

    string? Email { get; }
    UserRole? Role { get; }
    int? VendorId { get; }
    bool IsAuthenticated { get; }
    bool IsVendor { get; }
    bool IsAdmin { get; }
    bool IsCustomer { get; }
}
