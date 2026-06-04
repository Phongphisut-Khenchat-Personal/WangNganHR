using JanomeHR.Shared.Enums;

namespace JanomeHR.Shared.Entities;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationEvent Event { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string? Message { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Application Application { get; set; } = null!;
}