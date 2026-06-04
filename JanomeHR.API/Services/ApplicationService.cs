using JanomeHR.API.Data;
using JanomeHR.API.DTOs.Application;
using JanomeHR.API.Helpers;
using JanomeHR.API.Services.Interfaces;
using JanomeHR.Shared.Entities;
using JanomeHR.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace JanomeHR.API.Services;

public class ApplicationService(AppDbContext db) : IApplicationService
{
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
            .FirstOrDefaultAsync(a => a.Id == id);

        if (a is null) return null;

        return new ApplicationDetailDto(
            a.Id, a.ReferenceCode,
            a.JobPosting.Title, a.JobPosting.Department.Name,
            a.FirstName, a.LastName, a.Phone, a.Email,
            a.Birthdate, a.Address,
            a.EducationLevel.ToString(), a.EducationField,
            a.ExperienceYears, a.SalaryExpected, a.AvailableDate,
            a.Status.ToString(), a.Source.ToString(),
            a.Notes.Select(n => new NoteDto(
                n.Id, n.Content, n.Rating,
                n.CreatedByUser.FullName, n.CreatedAt)).ToList(),
            a.Documents.Select(d => new DocumentDto(
                d.Id, d.DocumentType.ToString(),
                d.FileName, d.FileUrl, d.UploadedAt)).ToList(),
            a.CreatedAt
        );
    }

    public async Task<ApplicationResponseDto> CreateAsync(CreateApplicationDto dto)
    {
        // สร้าง reference code ไม่ซ้ำ
        string refCode;
        do { refCode = ReferenceCodeHelper.Generate(); }
        while (await db.Applications.AnyAsync(a => a.ReferenceCode == refCode));

        if (!Enum.TryParse<EducationLevel>(dto.EducationLevel, out var edu))
            edu = EducationLevel.HighSchool;

        if (!Enum.TryParse<ApplicationSource>(dto.Source, out var src))
            src = ApplicationSource.Web;

        var app = new Application
        {
            JobPostingId   = dto.JobPostingId,
            ReferenceCode  = refCode,
            FirstName      = dto.FirstName,
            LastName       = dto.LastName,
            Phone          = dto.Phone,
            Email          = dto.Email,
            Birthdate      = dto.Birthdate,
            Address        = dto.Address,
            EducationLevel = edu,
            EducationField = dto.EducationField,
            ExperienceYears = dto.ExperienceYears,
            SalaryExpected = dto.SalaryExpected,
            AvailableDate  = dto.AvailableDate,
            Source         = src,
            Status         = ApplicationStatus.New,
        };

        db.Applications.Add(app);
        await db.SaveChangesAsync();
        await db.Entry(app).Reference(a => a.JobPosting).LoadAsync();

        return ToDto(app);
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

        return new TrackApplicationResponseDto(
            app.ReferenceCode,
            app.JobPosting.Title,
            app.Status.ToString(),
            app.CreatedAt
        );
    }

    private static ApplicationResponseDto ToDto(Application a) => new(
        a.Id, a.ReferenceCode,
        a.JobPosting?.Title ?? "",
        a.FirstName, a.LastName, a.Phone, a.Email,
        a.EducationLevel.ToString(), a.ExperienceYears,
        a.SalaryExpected, a.AvailableDate,
        a.Status.ToString(), a.Source.ToString(),
        a.CreatedAt
    );
}