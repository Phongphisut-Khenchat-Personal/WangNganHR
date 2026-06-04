namespace JanomeHR.Desktop.Models;

public record LoginRequest(string Username, string Password);

public record LoginResponse(
    string Token, string FullName,
    string Role, DateTime ExpiresAt);

public record JobPostingItem(
    Guid Id, string Title, string DepartmentName,
    string Description, string Requirements,
    decimal? SalaryMin, decimal? SalaryMax,
    int PositionsCount, string Status,
    DateTime? PublishedAt, DateTime? ClosedAt,
    string? QrCodeUrl, int TotalApplications,
    DateTime CreatedAt);

public record ApplicationItem(
    Guid Id, string ReferenceCode,
    string JobPostingTitle,
    string FirstName, string LastName,
    string Phone, string? Email,
    string EducationLevel, int ExperienceYears,
    decimal? SalaryExpected, DateOnly? AvailableDate,
    string Status, string Source,
    DateTime CreatedAt);

public record ApplicationDetail(
    Guid Id, string ReferenceCode,
    string JobPostingTitle, string DepartmentName,
    string FirstName, string LastName,
    string Phone, string? Email,
    DateOnly? Birthdate, string? Address,
    string EducationLevel, string? EducationField,
    int ExperienceYears, decimal? SalaryExpected,
    DateOnly? AvailableDate, string Status, string Source,
    List<NoteItem> Notes, List<DocumentItem> Documents,
    DateTime CreatedAt);

public record NoteItem(
    Guid Id, string Content, int? Rating,
    string CreatedByName, DateTime CreatedAt);

public record DocumentItem(
    Guid Id, string DocumentType,
    string FileName, string FileUrl,
    DateTime UploadedAt);

public record InterviewItem(
    Guid Id, string ApplicantName,
    string JobPostingTitle, string InterviewerName,
    DateTime ScheduledAt, int DurationMinutes,
    string Type, string? Location,
    string Status, string Result,
    DateTime CreatedAt);

public record CreateJobPostingRequest(
    int DepartmentId, string Title,
    string Description, string Requirements,
    decimal? SalaryMin, decimal? SalaryMax,
    int PositionsCount,
    DateTime? PublishedAt, DateTime? ClosedAt);

public record UpdateStatusRequest(
    string Status, string? Note, int? Rating);

public record CreateInterviewRequest(
    Guid ApplicationId, Guid InterviewerId,
    DateTime ScheduledAt, int DurationMinutes,
    string Type, string? Location);