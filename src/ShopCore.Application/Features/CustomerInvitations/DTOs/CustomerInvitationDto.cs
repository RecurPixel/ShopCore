namespace ShopCore.Application.CustomerInvitations.DTOs;

public record CustomerInvitationDto(
    int Id,
    int VendorId,
    string VendorName,
    string CustomerName,
    string PhoneNumber,
    string? Email,
    string DeliveryAddress,
    string Pincode,
    InvitationStatus Status,
    DateTime SentAt,
    DateTime ExpiresAt,
    DateTime? AcceptedAt
);

public record InvitationDetailsDto(
    string VendorName,
    string? VendorLogo,
    List<InvitationItemDto> Items,
    SubscriptionFrequency Frequency,
    string DeliveryTime,
    decimal MonthlyEstimate,
    decimal? DepositAmount
);

public record InvitationItemDto(
    int ProductId,
    string ProductName,
    string? ProductImageUrl,
    int Quantity,
    decimal UnitPrice,
    string Frequency
);

public record InvitationAcceptedDto(
    int UserId,
    int SubscriptionId,
    string Message,
    string? AppDownloadLink
);

public record InvitationItemInput(
    int ProductId,
    int Quantity
);
