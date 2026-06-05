namespace WangNganHR.API.DTOs.JobPosting;



public record CreateJobPostingDto(

    int DepartmentId,

    string Title,

    List<string> Responsibilities,

    List<string> Qualifications,

    List<string>? Benefits,

    string? WorkHours,

    string? WorkLocation,

    decimal? SalaryMin,

    decimal? SalaryMax,

    int PositionsCount,

    DateTime? PublishedAt,

    DateTime? ClosedAt

);



public record UpdateJobPostingDto(

    int DepartmentId,

    string Title,

    List<string> Responsibilities,

    List<string> Qualifications,

    List<string>? Benefits,

    string? WorkHours,

    string? WorkLocation,

    decimal? SalaryMin,

    decimal? SalaryMax,

    int PositionsCount,

    DateTime? ClosedAt

);



public record JobPostingResponseDto(

    Guid Id,

    int DepartmentId,

    string Title,

    string DepartmentName,

    List<string> Responsibilities,

    List<string> Qualifications,

    List<string> Benefits,

    string? WorkHours,

    string? WorkLocation,

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

