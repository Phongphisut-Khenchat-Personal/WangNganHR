namespace JanomeHR.API.DTOs.JobPosting;

public record CreateJobPostingDto(
    int DepartmentId,
    string Title,
    string Description,
    string Requirements,
    decimal? SalaryMin,
    decimal? SalaryMax,
    int PositionsCount,
    DateTime? PublishedAt,
    DateTime? ClosedAt
);

public record UpdateJobPostingDto(
    string Title,
    string Description,
    string Requirements,
    decimal? SalaryMin,
    decimal? SalaryMax,
    int PositionsCount,
    DateTime? ClosedAt
);

public record JobPostingResponseDto(
    Guid Id,
    string Title,
    string DepartmentName,
    string Description,
    string Requirements,
    decimal? SalaryMin,
    decimal? SalaryMax,
    int PositionsCount,
    string Status,
    DateTime? PublishedAt,
    DateTime? ClosedAt,
    string? QrCodeUrl,
    int TotalApplications,
    DateTime CreatedAt
);