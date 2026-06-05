namespace WangNganHR.Shared.Entities;

public class ApplicationNote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationId { get; set; }
    public Guid CreatedBy { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Application Application { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
}