namespace ShopCore.Domain.Entities;

public class SubscriptionItem
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public int ProductId { get; set; }

    // Item-specific settings
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Snapshot at subscription time
    public decimal? DiscountPercentage { get; set; }

    // Recurring/One-Time Support
    public bool IsRecurring { get; set; } = true;
    public DateTime? OneTimeDeliveryDate { get; set; }
    public bool IsDelivered { get; set; } = false;

    // Computed
    public decimal ItemTotal
    {
        get
        {
            var total = UnitPrice * Quantity;
            if (DiscountPercentage.HasValue)
                total -= total * (DiscountPercentage.Value / 100);
            return total;
        }
    }

    // Navigation
    public Subscription Subscription { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
