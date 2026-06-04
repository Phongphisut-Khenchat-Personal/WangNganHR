using JanomeHR.Shared.Enums;

namespace JanomeHR.Shared.Entities;

public class Interview
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationId { get; set; }
    public Guid InterviewerId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public InterviewType Type { get; set; }
    public string? Location { get; set; }
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    public InterviewResult Result { get; set; } = InterviewResult.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Application Application { get; set; } = null!;
    public User Interviewer { get; set; } = null!;
}