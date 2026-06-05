using WangNganHR.API.Data;
using WangNganHR.API.DTOs.Interview;
using WangNganHR.API.Services.Interfaces;
using WangNganHR.Shared.Entities;
using WangNganHR.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace WangNganHR.API.Services;

public class InterviewService(AppDbContext db) : IInterviewService
{
    public async Task<List<InterviewResponseDto>> GetAllAsync(DateTime? date)
    {
        var query = db.Interviews
            .Include(i => i.Application)
                .ThenInclude(a => a.JobPosting)
            .Include(i => i.Interviewer)
            .AsQueryable();

        if (date.HasValue)
        {
            var startUtc = DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc);
            var endUtc   = startUtc.AddDays(1);
            query = query.Where(i =>
                i.ScheduledAt >= startUtc && i.ScheduledAt < endUtc);
        }

        var list = await query.OrderBy(i => i.ScheduledAt).ToListAsync();

        return list.Select(i => ToDto(i)).ToList();
    }

    public async Task<InterviewResponseDto?> GetByIdAsync(Guid id)
    {
        var i = await db.Interviews
            .Include(i => i.Application)
                .ThenInclude(a => a.JobPosting)
            .Include(i => i.Interviewer)
            .FirstOrDefaultAsync(i => i.Id == id);

        return i is null ? null : ToDto(i);
    }

    public async Task<InterviewResponseDto> CreateAsync(CreateInterviewDto dto)
    {
        if (!Enum.TryParse<InterviewType>(dto.Type, out var type))
            type = InterviewType.Onsite;

        // แปลง ScheduledAt เป็น UTC
        var scheduledUtc = dto.ScheduledAt.Kind == DateTimeKind.Utc
            ? dto.ScheduledAt
            : DateTime.SpecifyKind(dto.ScheduledAt, DateTimeKind.Utc);

        var interview = new Interview
        {
            ApplicationId   = dto.ApplicationId,
            InterviewerId   = dto.InterviewerId,
            ScheduledAt     = scheduledUtc,
            DurationMinutes = dto.DurationMinutes,
            Type            = type,
            Location        = dto.Location,
            Status          = InterviewStatus.Scheduled,
            Result          = InterviewResult.Pending,
        };

        db.Interviews.Add(interview);

        // อัปเดตสถานะใบสมัครเป็น Interview
        var app = await db.Applications.FindAsync(dto.ApplicationId);
        if (app is not null)
        {
            app.Status    = ApplicationStatus.Interview;
            app.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        // reload with includes
        var saved = await db.Interviews
            .Include(i => i.Application)
                .ThenInclude(a => a.JobPosting)
            .Include(i => i.Interviewer)
            .FirstAsync(i => i.Id == interview.Id);

        return ToDto(saved);
    }

    public async Task<bool> UpdateResultAsync(Guid id, UpdateInterviewResultDto dto)
    {
        var interview = await db.Interviews
            .Include(i => i.Application)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (interview is null) return false;

        if (Enum.TryParse<InterviewResult>(dto.Result, out var result))
        {
            interview.Result = result;
            interview.Status = InterviewStatus.Done;
            interview.Notes  = dto.Notes;

            // sync สถานะใบสมัคร
            interview.Application.Status = result == InterviewResult.Pass
                ? ApplicationStatus.Pass
                : ApplicationStatus.Fail;
            interview.Application.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var interview = await db.Interviews.FindAsync(id);
        if (interview is null) return false;

        interview.Status = InterviewStatus.Cancelled;
        await db.SaveChangesAsync();
        return true;
    }

    private static InterviewResponseDto ToDto(Interview i) => new(
        i.Id,
        $"{i.Application.FirstName} {i.Application.LastName}",
        i.Application?.JobPosting?.Title ?? "",
        i.Interviewer?.FullName ?? "",
        i.ScheduledAt,
        i.DurationMinutes,
        i.Type.ToString(),
        i.Location,
        i.Status.ToString(),
        i.Result.ToString(),
        i.CreatedAt
    );
}