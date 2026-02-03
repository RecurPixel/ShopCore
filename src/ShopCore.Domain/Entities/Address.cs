namespace ShopCore.Domain.Entities;

public class Address : AuditableEntity
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = "India";
    public string Pincode { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    // Location fields
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? PlaceId { get; set; }
    public AddressType Type { get; set; } = AddressType.Home;
    public string? Landmark { get; set; }

    // Navigation
    public required User User { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
