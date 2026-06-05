using JanomeHR.API.Data;
using JanomeHR.API.DTOs.Application;
using JanomeHR.API.Helpers;
using JanomeHR.API.Services.Interfaces;
using JanomeHR.Shared.Entities;
using JanomeHR.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace JanomeHR.API.Services;

public class ApplicationService(AppDbContext db, IWebHostEnvironment env) : IApplicationService
{
    private const long MaxFileBytes = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

    public async Task<List<ApplicationResponseDto>> GetAllAsync(
        string? status, Guid? jobPostingId)
    {
        var query = db.Applications
            .Include(a => a.JobPosting)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) &&
            Enum.TryParse<ApplicationStatus>(status, out var s))
            query = query.Where(a => a.Status == s);

        if (jobPostingId.HasValue)
            query = query.Where(a => a.JobPostingId == jobPostingId);

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => ToDto(a))
            .ToListAsync();
    }

    public async Task<ApplicationDetailDto?> GetByIdAsync(Guid id)
    {
        var a = await db.Applications
            .Include(a => a.JobPosting).ThenInclude(j => j.Department)
            .Include(a => a.Notes).ThenInclude(n => n.CreatedByUser)
            .Include(a => a.Documents)
            .Include(a => a.WorkExperiences)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (a is null) return null;

        return ToDetailDto(a);
    }

    public async Task<ApplicationResponseDto> CreateAsync(CreateApplicationDto dto)
    {
        string refCode;
        do { refCode = ReferenceCodeHelper.Generate(); }
        while (await db.Applications.AnyAsync(a => a.ReferenceCode == refCode));

        var app = MapToEntity(dto, refCode);

        db.Applications.Add(app);
        await db.SaveChangesAsync();
        await db.Entry(app).Reference(a => a.JobPosting).LoadAsync();

        return ToDto(app);
    }

    public async Task<DocumentDto?> UploadDocumentAsync(
        Guid applicationId, string documentType, IFormFile file)
    {
        var app = await db.Applications.FindAsync(applicationId);
        if (app is null) return null;

        if (!Enum.TryParse<DocumentType>(documentType, out var docType))
            docType = DocumentType.Other;

        var ext = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(ext) || file.Length > MaxFileBytes)
            return null;

        var folder = Path.Combine(env.ContentRootPath, "uploads", applicationId.ToString());
        Directory.CreateDirectory(folder);

        var safeName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folder, safeName);

        await using (var stream = File.Create(fullPath))
            await file.CopyToAsync(stream);

        var doc = new ApplicationDocument
        {
            ApplicationId = applicationId,
            DocumentType  = docType,
            FileName      = file.FileName,
            FileUrl       = $"/uploads/{applicationId}/{safeName}"
        };

        db.ApplicationDocuments.Add(doc);
        await db.SaveChangesAsync();

        return new DocumentDto(
            doc.Id, doc.DocumentType.ToString(),
            doc.FileName, doc.FileUrl, doc.UploadedAt);
    }

    public async Task<bool> UpdateStatusAsync(
        Guid id, UpdateApplicationStatusDto dto, Guid userId)
    {
        var app = await db.Applications.FindAsync(id);
        if (app is null) return false;

        if (Enum.TryParse<ApplicationStatus>(dto.Status, out var status))
            app.Status = status;

        app.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(dto.Note))
        {
            db.ApplicationNotes.Add(new ApplicationNote
            {
                ApplicationId = id,
                CreatedBy     = userId,
                Content       = dto.Note,
                Rating        = dto.Rating,
            });
        }

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<TrackApplicationResponseDto?> TrackAsync(string referenceCode)
    {
        var app = await db.Applications
            .Include(a => a.JobPosting)
            .FirstOrDefaultAsync(a => a.ReferenceCode == referenceCode);

        if (app is null) return null;

        return ToTrackDto(app);
    }

    public async Task<List<TrackApplicationResponseDto>> TrackByIdentityAsync(string name, string phone)
    {
        var normalizedPhone = NormalizePhone(phone);
        if (string.IsNullOrWhiteSpace(name) || normalizedPhone.Length < 9)
            return [];

        var terms = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var query = db.Applications
            .Include(a => a.JobPosting)
            .AsQueryable();

        if (terms.Length >= 2)
        {
            var first = terms[0];
            var last = string.Join(' ', terms.Skip(1));
            query = query.Where(a =>
                EF.Functions.ILike(a.FirstName, $"%{first}%") &&
                EF.Functions.ILike(a.LastName, $"%{last}%"));
        }
        else
        {
            var term = terms[0];
            query = query.Where(a =>
                EF.Functions.ILike(a.FirstName, $"%{term}%") ||
                EF.Functions.ILike(a.LastName, $"%{term}%"));
        }

        var apps = await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return apps
            .Where(a => PhonesMatch(a.Phone, normalizedPhone))
            .Select(ToTrackDto)
            .ToList();
    }

    private static TrackApplicationResponseDto ToTrackDto(Application app) =>
        new(app.ReferenceCode, app.JobPosting.Title, app.Status.ToString(), app.CreatedAt);

    private static string NormalizePhone(string phone) =>
        new(phone.Where(char.IsDigit).ToArray());

    private static bool PhonesMatch(string stored, string normalizedInput)
    {
        var storedNorm = NormalizePhone(stored);
        if (storedNorm == normalizedInput) return true;
        if (storedNorm.Length >= 9 && normalizedInput.Length >= 9)
            return storedNorm[^9..] == normalizedInput[^9..];
        return false;
    }

    private static Application MapToEntity(CreateApplicationDto dto, string refCode)
    {
        Enum.TryParse<EducationLevel>(dto.EducationLevel, out var edu);
        Enum.TryParse<ApplicationSource>(dto.Source, out var src);
        var gender      = ParseEnum<Gender>(dto.Gender);
        var nationality = ParseEnum<NationalityType>(dto.NationalityType);
        var skillThai     = ParseEnum<SkillLevel>(dto.SkillThai);
        var skillEnglish  = ParseEnum<SkillLevel>(dto.SkillEnglish);
        var skillJapanese = ParseEnum<SkillLevel>(dto.SkillJapanese);

        var registered = dto.SameAsCurrentAddress ? dto.Address : dto.RegisteredAddress;

        var app = new Application
        {
            JobPostingId              = dto.JobPostingId,
            ReferenceCode             = refCode,
            TitlePrefix               = dto.TitlePrefix,
            FirstName                 = dto.FirstName,
            LastName                  = dto.LastName,
            Phone                     = dto.Phone,
            Email                     = dto.Email,
            NationalId                = dto.NationalId,
            Gender                    = gender,
            NationalityType           = nationality,
            LineId                    = dto.LineId,
            Birthdate                 = dto.Birthdate,
            Address                   = dto.Address,
            RegisteredAddress         = registered,
            SameAsCurrentAddress      = dto.SameAsCurrentAddress,
            Province                  = dto.Province,
            District                  = dto.District,
            PostalCode                = dto.PostalCode,
            EmergencyContactName      = dto.EmergencyContactName,
            EmergencyContactPhone     = dto.EmergencyContactPhone,
            EmergencyContactRelation  = dto.EmergencyContactRelation,
            EducationLevel            = edu,
            EducationField            = dto.EducationField,
            SchoolName                = dto.SchoolName,
            GraduationYear            = dto.GraduationYear,
            Gpa                       = dto.Gpa,
            SkillThai                 = skillThai,
            SkillEnglish              = skillEnglish,
            SkillJapanese             = skillJapanese,
            ComputerSkills            = dto.ComputerSkills,
            HasDriversLicense         = dto.HasDriversLicense,
            HasOwnVehicle             = dto.HasOwnVehicle,
            ExperienceYears           = dto.ExperienceYears,
            IsFreshGraduate           = dto.IsFreshGraduate,
            SalaryExpected            = dto.SalaryExpected,
            LastSalary                = dto.LastSalary,
            AvailableDate             = dto.AvailableDate,
            WillingShiftWork          = dto.WillingShiftWork,
            WillingOvertime           = dto.WillingOvertime,
            WillingRelocate           = dto.WillingRelocate,
            Source                    = src,
            ReferralSource            = dto.ReferralSource,
            PdpaConsentedAt           = dto.PdpaConsented ? DateTime.UtcNow : null,
            Status                    = ApplicationStatus.New,
        };

        if (!dto.IsFreshGraduate && dto.WorkExperiences is { Count: > 0 })
        {
            foreach (var w in dto.WorkExperiences)
            {
                app.WorkExperiences.Add(new WorkExperience
                {
                    CompanyName      = w.CompanyName,
                    Position         = w.Position,
                    StartDate        = w.StartDate,
                    EndDate          = w.EndDate,
                    Responsibilities = w.Responsibilities,
                    ReasonForLeaving = w.ReasonForLeaving,
                    Salary           = w.Salary,
                });
            }
        }

        return app;
    }

    private static TEnum? ParseEnum<TEnum>(string? value) where TEnum : struct, Enum =>
        value is not null && Enum.TryParse<TEnum>(value, out var result) ? result : null;

    private static ApplicationResponseDto ToDto(Application a) => new(
        a.Id, a.ReferenceCode,
        a.JobPosting?.Title ?? "",
        a.FirstName, a.LastName, a.Phone, a.Email,
        a.EducationLevel.ToString(), a.ExperienceYears,
        a.SalaryExpected, a.AvailableDate,
        a.Status.ToString(), a.Source.ToString(),
        a.CreatedAt
    );

    private static ApplicationDetailDto ToDetailDto(Application a) => new(
        a.Id, a.ReferenceCode,
        a.JobPosting.Title, a.JobPosting.Department.Name,
        a.TitlePrefix,
        a.FirstName, a.LastName, a.Phone, a.Email,
        a.NationalId,
        a.Gender?.ToString(), a.NationalityType?.ToString(), a.LineId,
        a.Birthdate, a.Address, a.RegisteredAddress, a.SameAsCurrentAddress,
        a.Province, a.District, a.PostalCode,
        a.EmergencyContactName, a.EmergencyContactPhone, a.EmergencyContactRelation,
        a.EducationLevel.ToString(), a.EducationField,
        a.SchoolName, a.GraduationYear, a.Gpa,
        a.SkillThai?.ToString(), a.SkillEnglish?.ToString(), a.SkillJapanese?.ToString(),
        a.ComputerSkills, a.HasDriversLicense, a.HasOwnVehicle,
        a.ExperienceYears, a.IsFreshGraduate,
        a.SalaryExpected, a.LastSalary, a.AvailableDate,
        a.WillingShiftWork, a.WillingOvertime, a.WillingRelocate,
        a.Status.ToString(), a.Source.ToString(),
        a.ReferralSource, a.PdpaConsentedAt,
        a.WorkExperiences.Select(w => new WorkExperienceDto(
            w.CompanyName, w.Position, w.StartDate, w.EndDate,
            w.Responsibilities, w.ReasonForLeaving, w.Salary)).ToList(),
        a.Notes.Select(n => new NoteDto(
            n.Id, n.Content, n.Rating,
            n.CreatedByUser.FullName, n.CreatedAt)).ToList(),
        a.Documents.Select(d => new DocumentDto(
            d.Id, d.DocumentType.ToString(),
            d.FileName, d.FileUrl, d.UploadedAt)).ToList(),
        a.CreatedAt
    );
}
