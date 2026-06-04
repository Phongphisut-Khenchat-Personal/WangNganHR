using System.Net.Http.Json;
using JanomeHR.Shared.Enums;

namespace JanomeHR.Web.Services;

public class ApiService(HttpClient http, IConfiguration config)
{
    private readonly string _base = config["ApiBaseUrl"] ?? "http://localhost:5083";

    // ── Job Postings ──────────────────────────────
    public async Task<List<JobPostingItem>> GetActiveJobsAsync()
    {
        var result = await http.GetFromJsonAsync<List<JobPostingItem>>(
            $"{_base}/api/JobPostings/active");
        return result ?? [];
    }

    public async Task<JobPostingItem?> GetJobByIdAsync(string id)
    {
        return await http.GetFromJsonAsync<JobPostingItem>(
            $"{_base}/api/JobPostings/{id}");
    }

    // ── Applications ──────────────────────────────
    public async Task<ApplicationResult?> SubmitApplicationAsync(
        ApplicationForm form)
    {
        var res = await http.PostAsJsonAsync(
            $"{_base}/api/Applications", form);

        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<ApplicationResult>();
    }

    public async Task<TrackResult?> TrackApplicationAsync(string refCode)
    {
        return await http.GetFromJsonAsync<TrackResult>(
            $"{_base}/api/Applications/track/{refCode}");
    }
}

// ── DTOs สำหรับ Blazor ───────────────────────────
public record JobPostingItem(
    Guid Id, string Title, string DepartmentName,
    string Description, string Requirements,
    decimal? SalaryMin, decimal? SalaryMax,
    int PositionsCount, string Status,
    DateTime? PublishedAt, DateTime? ClosedAt,
    string? QrCodeUrl, int TotalApplications,
    DateTime CreatedAt);

public record ApplicationForm(
    Guid JobPostingId,
    string FirstName, string LastName,
    string Phone, string? Email,
    DateOnly? Birthdate, string? Address,
    string EducationLevel, string? EducationField,
    int ExperienceYears, decimal? SalaryExpected,
    DateOnly? AvailableDate, string Source);

public record ApplicationResult(
    Guid Id, string ReferenceCode,
    string JobPostingTitle, string Status,
    DateTime CreatedAt);

public record TrackResult(
    string ReferenceCode, string JobPostingTitle,
    string Status, DateTime CreatedAt);