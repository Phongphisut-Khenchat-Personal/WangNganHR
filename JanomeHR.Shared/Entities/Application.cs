using JanomeHR.Shared.Enums;



namespace JanomeHR.Shared.Entities;



public class Application

{

    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobPostingId { get; set; }

    public string ReferenceCode { get; set; } = string.Empty;

    public string? TitlePrefix { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? NationalId { get; set; }

    public Gender? Gender { get; set; }

    public NationalityType? NationalityType { get; set; }

    public string? LineId { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string? Address { get; set; }

    public string? RegisteredAddress { get; set; }

    public bool SameAsCurrentAddress { get; set; }

    public string? Province { get; set; }

    public string? District { get; set; }

    public string? PostalCode { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public string? EmergencyContactRelation { get; set; }

    public EducationLevel EducationLevel { get; set; }

    public string? EducationField { get; set; }

    public string? SchoolName { get; set; }

    public int? GraduationYear { get; set; }

    public decimal? Gpa { get; set; }

    public SkillLevel? SkillThai { get; set; }

    public SkillLevel? SkillEnglish { get; set; }

    public SkillLevel? SkillJapanese { get; set; }

    public string? ComputerSkills { get; set; }

    public bool? HasDriversLicense { get; set; }

    public bool? HasOwnVehicle { get; set; }

    public int ExperienceYears { get; set; } = 0;

    public bool IsFreshGraduate { get; set; }

    public decimal? SalaryExpected { get; set; }

    public decimal? LastSalary { get; set; }

    public DateOnly? AvailableDate { get; set; }

    public bool? WillingShiftWork { get; set; }

    public bool? WillingOvertime { get; set; }

    public bool? WillingRelocate { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.New;

    public ApplicationSource Source { get; set; } = ApplicationSource.Web;

    public string? ReferralSource { get; set; }

    public DateTime? PdpaConsentedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;



    public JobPosting JobPosting { get; set; } = null!;

    public ICollection<ApplicationDocument> Documents { get; set; } = [];

    public ICollection<ApplicationNote> Notes { get; set; } = [];

    public ICollection<Interview> Interviews { get; set; } = [];

    public ICollection<Notification> Notifications { get; set; } = [];

    public ICollection<WorkExperience> WorkExperiences { get; set; } = [];

}

