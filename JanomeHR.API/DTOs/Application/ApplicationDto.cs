namespace JanomeHR.API.DTOs.Application;

public record CreateApplicationDto(
    Guid JobPostingId,
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    DateOnly? Birthdate,
    string? Address,
    string EducationLevel,
    string? EducationField,
    int ExperienceYears,
    decimal? SalaryExpected,
    DateOnly? AvailableDate,
    string Source
);

public record UpdateApplicationStatusDto(
    string Status,
    string? Note,
    int? Rating
);

public record ApplicationResponseDto(
    Guid Id,
    string ReferenceCode,
    string JobPostingTitle,
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    string EducationLevel,
    int ExperienceYears,
    decimal? SalaryExpected,
    DateOnly? AvailableDate,
    string Status,
    string Source,
    DateTime CreatedAt
);

public record ApplicationDetailDto(
    Guid Id,
    string ReferenceCode,
    string JobPostingTitle,
    string DepartmentName,
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    DateOnly? Birthdate,
    string? Address,
    string EducationLevel,
    string? EducationField,
    int ExperienceYears,
    decimal? SalaryExpected,
    DateOnly? AvailableDate,
    string Status,
    string Source,
    List<NoteDto> Notes,
    List<DocumentDto> Documents,
    DateTime CreatedAt
);

public record NoteDto(
    Guid Id,
    string Content,
    int? Rating,
    string CreatedByName,
    DateTime CreatedAt
);

public record DocumentDto(
    Guid Id,
    string DocumentType,
    string FileName,
    string FileUrl,
    DateTime UploadedAt
);

public record TrackApplicationDto(string ReferenceCode);

public record TrackApplicationResponseDto(
    string ReferenceCode,
    string JobPostingTitle,
    string Status,
    DateTime CreatedAt
);