namespace JanomeHR.Shared.Enums;

public enum UserRole
{
    Admin,
    HR,
    Manager
}

public enum JobPostingStatus
{
    Draft,
    Active,
    Closed
}

public enum ApplicationStatus
{
    New,
    Review,
    Interview,
    Pass,
    Fail
}

public enum ApplicationSource
{
    QRCode,
    WalkIn,
    Web,
    Other
}

public enum EducationLevel
{
    BelowMiddleSchool,
    MiddleSchool,
    HighSchool,
    Vocational,        // ปวช.
    HighVocational,    // ปวส.
    Bachelor,
    Master,
    Doctor
}

public enum InterviewType
{
    Online,
    Onsite
}

public enum InterviewStatus
{
    Scheduled,
    Done,
    Cancelled
}

public enum InterviewResult
{
    Pending,
    Pass,
    Fail
}

public enum NotificationType
{
    Email,
    LINE,
    SMS
}

public enum NotificationEvent
{
    NewApplication,
    InterviewScheduled,
    InterviewReminder,
    ResultPass,
    ResultFail
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed
}

public enum DocumentType
{
    Resume,
    Photo,
    Certificate,
    Other
}

public enum Gender
{
    Male,
    Female,
    Other
}

public enum NationalityType
{
    Thai,
    Foreign,
    MigrantWorker
}

public enum SkillLevel
{
    None,
    Basic,
    Good,
    Fluent
}