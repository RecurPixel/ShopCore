using ShopCore.Domain.Enums;

namespace ShopCore.Api.Models;

public class DeliveryItemStatusRequest
{
    public int DeliveryItemId { get; set; }
    public DeliveryItemStatus Status { get; set; }
    public string? Notes { get; set; }
}
