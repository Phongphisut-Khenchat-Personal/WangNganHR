using JanomeHR.Shared.Enums;

namespace JanomeHR.Shared.Entities;

public class JobPosting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int DepartmentId { get; set; }
    public Guid CreatedBy { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public int PositionsCount { get; set; } = 1;
    public JobPostingStatus Status { get; set; } = JobPostingStatus.Draft;
    public DateTime? PublishedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? QrCodeUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Department Department { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    public ICollection<Application> Applications { get; set; } = [];
}