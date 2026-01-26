using Microsoft.AspNetCore.Http;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Domain.Enums;
using System.Security.Claims;


namespace ShopCore.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userIdClaim, out var id) ? id : null;
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

    public UserRole? Role
    {
        get
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Role)?.Value;

            return Enum.TryParse<UserRole>(roleClaim, out var role) ? role : null;
        }
    }

    public int? VendorId
    {
        get
        {
            var vendorIdClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirst("VendorId")?.Value;

            return int.TryParse(vendorIdClaim, out var id) ? id : null;
        }
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsVendor => Role == UserRole.Vendor;

    public bool IsAdmin => Role == UserRole.Admin;

    public bool IsCustomer => Role == UserRole.Customer;
}