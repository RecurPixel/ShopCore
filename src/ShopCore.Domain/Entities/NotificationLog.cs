namespace ShopCore.Domain.Entities;

public class NotificationLog : BaseEntity
{
    public string Channel { get; set; } = string.Empty;
    public string? Provider { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ProviderId { get; set; }
    public string? Error { get; set; }
    public DateTime SentAt { get; set; }
}
