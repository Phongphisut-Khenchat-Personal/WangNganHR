using WangNganHR.API.Data;

using WangNganHR.API.DTOs.JobPosting;

using WangNganHR.API.Services.Interfaces;

using WangNganHR.Shared.Entities;

using WangNganHR.Shared.Enums;

using Microsoft.EntityFrameworkCore;

using QRCoder;



namespace WangNganHR.API.Services;



public class JobPostingService(AppDbContext db) : IJobPostingService

{

    public async Task<List<JobPostingResponseDto>> GetAllAsync()

    {

        return await db.JobPostings

            .Include(j => j.Department)

            .Include(j => j.Applications)

            .OrderByDescending(j => j.CreatedAt)

            .Select(j => ToDto(j))

            .ToListAsync();

    }



    public async Task<List<JobPostingResponseDto>> GetActiveAsync()

    {

        return await db.JobPostings

            .Include(j => j.Department)

            .Include(j => j.Applications)

            .Where(j => j.Status == JobPostingStatus.Active)

            .OrderByDescending(j => j.PublishedAt)

            .Select(j => ToDto(j))

            .ToListAsync();

    }



    public async Task<JobPostingResponseDto?> GetByIdAsync(Guid id)

    {

        var job = await db.JobPostings

            .Include(j => j.Department)

            .Include(j => j.Applications)

            .FirstOrDefaultAsync(j => j.Id == id);



        return job is null ? null : ToDto(job);

    }



    public async Task<JobPostingResponseDto> CreateAsync(CreateJobPostingDto dto, Guid userId)

    {

        var responsibilities = NormalizeList(dto.Responsibilities);

        var qualifications = NormalizeList(dto.Qualifications);

        var benefits = NormalizeList(dto.Benefits);



        var job = new JobPosting

        {

            DepartmentId     = dto.DepartmentId,

            CreatedBy        = userId,

            Title            = dto.Title.Trim(),

            Responsibilities = responsibilities,

            Qualifications   = qualifications,

            Benefits         = benefits,

            Description      = JoinLines(responsibilities),

            Requirements     = JoinLines(qualifications),

            WorkHours        = TrimOrNull(dto.WorkHours),

            WorkLocation     = TrimOrNull(dto.WorkLocation),

            SalaryMin        = dto.SalaryMin,

            SalaryMax        = dto.SalaryMax,

            PositionsCount   = dto.PositionsCount,

            Status           = dto.PublishedAt is null

                                   ? JobPostingStatus.Draft

                                   : JobPostingStatus.Active,

            PublishedAt      = dto.PublishedAt,

            ClosedAt         = dto.ClosedAt,

        };



        db.JobPostings.Add(job);

        await db.SaveChangesAsync();



        await db.Entry(job).Reference(j => j.Department).LoadAsync();

        return ToDto(job);

    }



    public async Task<JobPostingResponseDto?> UpdateAsync(Guid id, UpdateJobPostingDto dto)

    {

        var job = await db.JobPostings

            .Include(j => j.Department)

            .Include(j => j.Applications)

            .FirstOrDefaultAsync(j => j.Id == id);



        if (job is null) return null;



        var responsibilities = NormalizeList(dto.Responsibilities);

        var qualifications = NormalizeList(dto.Qualifications);

        var benefits = NormalizeList(dto.Benefits);



        job.DepartmentId     = dto.DepartmentId;
        job.Title            = dto.Title.Trim();

        job.Responsibilities = responsibilities;

        job.Qualifications   = qualifications;

        job.Benefits         = benefits;

        job.Description      = JoinLines(responsibilities);

        job.Requirements     = JoinLines(qualifications);

        job.WorkHours        = TrimOrNull(dto.WorkHours);

        job.WorkLocation     = TrimOrNull(dto.WorkLocation);

        job.SalaryMin        = dto.SalaryMin;

        job.SalaryMax        = dto.SalaryMax;

        job.PositionsCount   = dto.PositionsCount;

        job.ClosedAt         = dto.ClosedAt;

        job.UpdatedAt        = DateTime.UtcNow;



        await db.SaveChangesAsync();

        return ToDto(job);

    }



    public async Task<bool> PublishAsync(Guid id)

    {

        var job = await db.JobPostings.FindAsync(id);

        if (job is null || job.Status != JobPostingStatus.Draft) return false;



        job.Status      = JobPostingStatus.Active;

        job.PublishedAt = DateTime.UtcNow;

        job.UpdatedAt   = DateTime.UtcNow;



        await db.SaveChangesAsync();

        return true;

    }



    public async Task<bool> CloseAsync(Guid id)

    {

        var job = await db.JobPostings.FindAsync(id);

        if (job is null || job.Status != JobPostingStatus.Active) return false;



        job.Status    = JobPostingStatus.Closed;

        job.ClosedAt  = DateTime.UtcNow;

        job.UpdatedAt = DateTime.UtcNow;



        await db.SaveChangesAsync();

        return true;

    }



    public async Task<string?> GenerateQrCodeAsync(Guid id, string baseUrl)

    {

        var job = await db.JobPostings.FindAsync(id);

        if (job is null) return null;



        var url = $"{baseUrl}/apply/{id}";



        using var qrGenerator = new QRCodeGenerator();

        var qrData  = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

        var qrCode  = new PngByteQRCode(qrData);

        var qrBytes = qrCode.GetGraphic(10);

        var base64  = Convert.ToBase64String(qrBytes);

        var dataUrl = $"data:image/png;base64,{base64}";



        job.QrCodeUrl = dataUrl;

        job.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();



        return dataUrl;

    }



    private static JobPostingResponseDto ToDto(JobPosting j) => new(

        j.Id,

        j.DepartmentId,

        j.Title,

        j.Department?.Name ?? "",

        ResolveList(j.Responsibilities, j.Description),

        ResolveList(j.Qualifications, j.Requirements),

        j.Benefits ?? [],

        j.WorkHours,

        j.WorkLocation,

        j.SalaryMin,

        j.SalaryMax,

        j.PositionsCount,

        j.Status.ToString(),

        j.PublishedAt,

        j.ClosedAt,

        j.QrCodeUrl,

        j.Applications?.Count ?? 0,

        j.CreatedAt

    );



    private static List<string> NormalizeList(IEnumerable<string>? items) =>

        items?

            .Select(x => x.Trim())

            .Where(x => !string.IsNullOrWhiteSpace(x))

            .ToList() ?? [];



    private static string JoinLines(IReadOnlyList<string> items) =>

        items.Count == 0 ? string.Empty : string.Join('\n', items);



    private static string? TrimOrNull(string? value) =>

        string.IsNullOrWhiteSpace(value) ? null : value.Trim();



    private static List<string> ResolveList(List<string> items, string legacy)

    {

        if (items.Count > 0) return items;

        return ParseLegacyLines(legacy);

    }



    private static List<string> ParseLegacyLines(string? text)

    {

        if (string.IsNullOrWhiteSpace(text)) return [];



        return text

            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)

            .Select(l => l.TrimStart('-', '•', '*', ' ').Trim())

            .Where(l => !string.IsNullOrWhiteSpace(l))

            .ToList();

    }

}

