namespace ShopCore.Application.Vendors.DTOs;

public record VendorProfileDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string BusinessName { get; init; } = string.Empty;
    public string? BusinessDescription { get; init; }
    public string? BusinessLogo { get; init; }
    public string? BusinessAddress { get; init; }
    public string? GstNumber { get; init; }
    public string? PanNumber { get; init; }
    public string? BankName { get; init; }
    public string? BankAccountNumber { get; init; }
    public string? BankIfscCode { get; init; }
    public string? BankAccountHolderName { get; init; }
    public bool RequiresDeposit { get; init; }
    public decimal? DefaultDepositAmount { get; init; }
    public int? DefaultBillingCycleDays { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Rating { get; init; }
    public int TotalReviews { get; init; }
    public int TotalProducts { get; init; }
    public int TotalOrders { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public int DaysSinceSubmission { get; init; }
    public DateTime CreatedAt { get; init; }

    public DateTime? ApprovedAt { get; set; }

    public decimal CommissionRate { get; set; } = 5.00m; // percentage (5.00 = 5%)
    public decimal TotalRevenue { get; set; } = 0;
}
