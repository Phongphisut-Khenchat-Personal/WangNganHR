namespace JanomeHR.API.DTOs.Application;



public record WorkExperienceDto(

    string CompanyName,

    string Position,

    DateOnly? StartDate,

    DateOnly? EndDate,

    string? Responsibilities,

    string? ReasonForLeaving,

    decimal? Salary

);



public record CreateApplicationDto(

    Guid JobPostingId,

    string? TitlePrefix,

    string FirstName,

    string LastName,

    string Phone,

    string? Email,

    string? NationalId,

    string? Gender,

    string? NationalityType,

    string? LineId,

    DateOnly? Birthdate,

    string? Address,

    string? RegisteredAddress,

    bool SameAsCurrentAddress,

    string? Province,

    string? District,

    string? PostalCode,

    string? EmergencyContactName,

    string? EmergencyContactPhone,

    string? EmergencyContactRelation,

    string EducationLevel,

    string? EducationField,

    string? SchoolName,

    int? GraduationYear,

    decimal? Gpa,

    string? SkillThai,

    string? SkillEnglish,

    string? SkillJapanese,

    string? ComputerSkills,

    bool? HasDriversLicense,

    bool? HasOwnVehicle,

    int ExperienceYears,

    bool IsFreshGraduate,

    decimal? SalaryExpected,

    decimal? LastSalary,

    DateOnly? AvailableDate,

    bool? WillingShiftWork,

    bool? WillingOvertime,

    bool? WillingRelocate,

    string Source,

    string? ReferralSource,

    bool PdpaConsented,

    List<WorkExperienceDto>? WorkExperiences

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

    string? TitlePrefix,

    string FirstName,

    string LastName,

    string Phone,

    string? Email,

    string? NationalId,

    string? Gender,

    string? NationalityType,

    string? LineId,

    DateOnly? Birthdate,

    string? Address,

    string? RegisteredAddress,

    bool SameAsCurrentAddress,

    string? Province,

    string? District,

    string? PostalCode,

    string? EmergencyContactName,

    string? EmergencyContactPhone,

    string? EmergencyContactRelation,

    string EducationLevel,

    string? EducationField,

    string? SchoolName,

    int? GraduationYear,

    decimal? Gpa,

    string? SkillThai,

    string? SkillEnglish,

    string? SkillJapanese,

    string? ComputerSkills,

    bool? HasDriversLicense,

    bool? HasOwnVehicle,

    int ExperienceYears,

    bool IsFreshGraduate,

    decimal? SalaryExpected,

    decimal? LastSalary,

    DateOnly? AvailableDate,

    bool? WillingShiftWork,

    bool? WillingOvertime,

    bool? WillingRelocate,

    string Status,

    string Source,

    string? ReferralSource,

    DateTime? PdpaConsentedAt,

    List<WorkExperienceDto> WorkExperiences,

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

