namespace WangNganHR.API.DTOs.Interview;

public record CreateInterviewDto(
    Guid ApplicationId,
    Guid InterviewerId,
    DateTime ScheduledAt,
    int DurationMinutes,
    string Type,
    string? Location
);

public record UpdateInterviewResultDto(
    string Result,
    string? Notes
);

public record InterviewResponseDto(
    Guid Id,
    string ApplicantName,
    string JobPostingTitle,
    string InterviewerName,
    DateTime ScheduledAt,
    int DurationMinutes,
    string Type,
    string? Location,
    string Status,
    string Result,
    DateTime CreatedAt
);