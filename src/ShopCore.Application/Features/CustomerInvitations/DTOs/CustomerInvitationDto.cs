namespace ShopCore.Application.CustomerInvitations.DTOs;

public record CustomerInvitationDto
{
    public int Id { get; init; }
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string DeliveryAddress { get; init; } = string.Empty;
    public string Pincode { get; init; } = string.Empty;
    public InvitationStatus Status { get; init; }
    public DateTime SentAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public DateTime? AcceptedAt { get; init; }
}

public record InvitationDetailsDto
{
    public string VendorName { get; init; } = string.Empty;
    public string? VendorLogo { get; init; }
    public List<InvitationItemDto> Items { get; init; } = new();
    public SubscriptionFrequency Frequency { get; init; }
    public string DeliveryTime { get; init; } = string.Empty;
    public decimal MonthlyEstimate { get; init; }
    public decimal? DepositAmount { get; init; }
}

public record InvitationItemDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImageUrl { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string Frequency { get; init; } = string.Empty;
}

public record InvitationAcceptedDto
{
    public int UserId { get; init; }
    public int SubscriptionId { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? AppDownloadLink { get; init; }
}

public record InvitationItemInput
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
};