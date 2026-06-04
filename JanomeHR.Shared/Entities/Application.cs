using JanomeHR.Shared.Enums;

namespace JanomeHR.Shared.Entities;

public class Application
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobPostingId { get; set; }
    public string ReferenceCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateOnly? Birthdate { get; set; }
    public string? Address { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public string? EducationField { get; set; }
    public int ExperienceYears { get; set; } = 0;
    public decimal? SalaryExpected { get; set; }
    public DateOnly? AvailableDate { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.New;
    public ApplicationSource Source { get; set; } = ApplicationSource.Web;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public JobPosting JobPosting { get; set; } = null!;
    public ICollection<ApplicationDocument> Documents { get; set; } = [];
    public ICollection<ApplicationNote> Notes { get; set; } = [];
    public ICollection<Interview> Interviews { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
}