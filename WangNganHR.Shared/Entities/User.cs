using WangNganHR.Shared.Enums;

namespace WangNganHR.Shared.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }

    // Navigation
    public ICollection<JobPosting> JobPostings { get; set; } = [];
    public ICollection<Interview> Interviews { get; set; } = [];
    public ICollection<ApplicationNote> ApplicationNotes { get; set; } = [];
}