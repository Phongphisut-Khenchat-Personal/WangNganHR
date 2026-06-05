namespace JanomeHR.Desktop.Models;

public record LoginRequest(string Username, string Password);

public record LoginResponse(
    string Token, string FullName,
    string Role, DateTime ExpiresAt);

public record JobPostingItem(
    Guid Id, int DepartmentId, string Title, string DepartmentName,
    List<string> Responsibilities,
    List<string> Qualifications,
    List<string> Benefits,
    string? WorkHours,
    string? WorkLocation,
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

public record WorkExperienceItem(
    string CompanyName, string Position,
    DateOnly? StartDate, DateOnly? EndDate,
    string? Responsibilities, string? ReasonForLeaving,
    decimal? Salary);

public record ApplicationDetail(
    Guid Id, string ReferenceCode,
    string JobPostingTitle, string DepartmentName,
    string? TitlePrefix,
    string FirstName, string LastName,
    string Phone, string? Email,
    string? NationalId, string? Gender, string? NationalityType, string? LineId,
    DateOnly? Birthdate, string? Address,
    string? RegisteredAddress, bool SameAsCurrentAddress,
    string? Province, string? District, string? PostalCode,
    string? EmergencyContactName, string? EmergencyContactPhone, string? EmergencyContactRelation,
    string EducationLevel, string? EducationField,
    string? SchoolName, int? GraduationYear, decimal? Gpa,
    string? SkillThai, string? SkillEnglish, string? SkillJapanese, string? ComputerSkills,
    bool? HasDriversLicense, bool? HasOwnVehicle,
    int ExperienceYears, bool IsFreshGraduate,
    decimal? SalaryExpected, decimal? LastSalary,
    DateOnly? AvailableDate,
    bool? WillingShiftWork, bool? WillingOvertime, bool? WillingRelocate,
    string Status, string Source,
    string? ReferralSource, DateTime? PdpaConsentedAt,
    List<WorkExperienceItem> WorkExperiences,
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
    List<string> Responsibilities,
    List<string> Qualifications,
    List<string>? Benefits,
    string? WorkHours,
    string? WorkLocation,
    decimal? SalaryMin, decimal? SalaryMax,
    int PositionsCount,
    DateTime? PublishedAt, DateTime? ClosedAt);

public record UpdateJobPostingRequest(
    int DepartmentId, string Title,
    List<string> Responsibilities,
    List<string> Qualifications,
    List<string>? Benefits,
    string? WorkHours,
    string? WorkLocation,
    decimal? SalaryMin, decimal? SalaryMax,
    int PositionsCount);

public record UpdateStatusRequest(
    string Status, string? Note, int? Rating);

public record CreateInterviewRequest(
    Guid ApplicationId, Guid InterviewerId,
    DateTime ScheduledAt, int DurationMinutes,
    string Type, string? Location);