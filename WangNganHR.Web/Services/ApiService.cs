using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WangNganHR.Web.Services;

public class ApiService(HttpClient http, IConfiguration config)
{
    private readonly string _base = config["ApiBaseUrl"] ?? "http://localhost:5083";

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

    public async Task<ApplicationResult?> SubmitApplicationAsync(ApplicationForm form)
    {
        var res = await http.PostAsJsonAsync($"{_base}/api/Applications", form);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<ApplicationResult>();
    }

    public async Task<bool> UploadDocumentAsync(
        Guid applicationId, string documentType, Stream stream, string fileName)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(documentType), "documentType");

        var res = await http.PostAsync(
            $"{_base}/api/Applications/{applicationId}/documents", content);
        return res.IsSuccessStatusCode;
    }

    public async Task<TrackResult?> TrackApplicationAsync(string refCode)
    {
        var res = await http.GetAsync(
            $"{_base}/api/Applications/track/{Uri.EscapeDataString(refCode)}");
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<TrackResult>();
    }

    public async Task<List<TrackResult>> TrackByIdentityAsync(string name, string phone)
    {
        var q = $"name={Uri.EscapeDataString(name)}&phone={Uri.EscapeDataString(phone)}";
        var res = await http.GetAsync($"{_base}/api/Applications/track?{q}");
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return [];
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<List<TrackResult>>() ?? [];
    }
}

public record JobPostingItem(
    Guid Id, int DepartmentId, string Title, string DepartmentName,
    List<string> Responsibilities,
    List<string> Qualifications,
    List<string> Benefits,
    string? WorkHours,
    string? WorkLocation,
    decimal? SalaryMin, decimal? SalaryMax,
    int PositionsCount, string Status,
    DateTime? PublishedAt, DateTime? ClosedAt,
    string? QrCodeUrl, int TotalApplications,
    DateTime CreatedAt);

public record WorkExperienceForm(
    string CompanyName,
    string Position,
    DateOnly? StartDate,
    DateOnly? EndDate,
    string? Responsibilities,
    string? ReasonForLeaving,
    decimal? Salary);

public record ApplicationForm(
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
    List<WorkExperienceForm>? WorkExperiences);

public record ApplicationResult(
    Guid Id, string ReferenceCode,
    string JobPostingTitle, string Status,
    DateTime CreatedAt);

public record TrackResult(
    string ReferenceCode, string JobPostingTitle,
    string Status, DateTime CreatedAt);
