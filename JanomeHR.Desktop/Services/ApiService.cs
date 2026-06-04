using JanomeHR.Desktop.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace JanomeHR.Desktop.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private const string Base = "http://localhost:5083";
    public string? CurrentUserName { get; private set; }
    public string? CurrentRole { get; private set; }
    public Guid? CurrentUserId { get; private set; }

    public ApiService()
    {
        _http = new HttpClient();
    }

    public void SetToken(string token)
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // decode JWT payload
        var parts  = token.Split('.');
        if (parts.Length < 2) return;
        var payload = parts[1];
        var padded  = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
        var json    = Encoding.UTF8.GetString(Convert.FromBase64String(padded));
        var claims  = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        if (claims is null) return;

        CurrentUserName = claims.GetValueOrDefault(
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.ToString();
        CurrentRole = claims.GetValueOrDefault(
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.ToString();

        var idClaim = claims.GetValueOrDefault(
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.ToString();
        if (Guid.TryParse(idClaim, out var uid))
            CurrentUserId = uid;
    }

    // ── Auth ─────────────────────────────────────
    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        var res = await _http.PostAsJsonAsync($"{Base}/api/Auth/login",
            new LoginRequest(username, password));
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<LoginResponse>();
    }

    // ── Job Postings ──────────────────────────────
    public async Task<List<JobPostingItem>> GetJobPostingsAsync()
    {
        var result = await _http.GetFromJsonAsync<List<JobPostingItem>>(
            $"{Base}/api/JobPostings");
        return result ?? [];
    }

    public async Task<JobPostingItem?> CreateJobPostingAsync(
        CreateJobPostingRequest req)
    {
        var res = await _http.PostAsJsonAsync(
            $"{Base}/api/JobPostings", req);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<JobPostingItem>();
    }

    public async Task<bool> PublishJobPostingAsync(Guid id)
    {
        var res = await _http.PostAsync(
            $"{Base}/api/JobPostings/{id}/publish", null);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> CloseJobPostingAsync(Guid id)
    {
        var res = await _http.PostAsync(
            $"{Base}/api/JobPostings/{id}/close", null);
        return res.IsSuccessStatusCode;
    }

    public async Task<string?> GenerateQrCodeAsync(Guid id)
    {
        var res = await _http.PostAsync(
            $"{Base}/api/JobPostings/{id}/qrcode", null);
        if (!res.IsSuccessStatusCode) return null;
        var data = await res.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return data?.GetValueOrDefault("qrCode");
    }

    // ── Applications ──────────────────────────────
    public async Task<List<ApplicationItem>> GetApplicationsAsync(
        string? status = null, Guid? jobPostingId = null)
    {
        var url = $"{Base}/api/Applications";
        var qs  = new List<string>();
        if (!string.IsNullOrEmpty(status)) qs.Add($"status={status}");
        if (jobPostingId.HasValue) qs.Add($"jobPostingId={jobPostingId}");
        if (qs.Any()) url += "?" + string.Join("&", qs);

        var result = await _http.GetFromJsonAsync<List<ApplicationItem>>(url);
        return result ?? [];
    }

    public async Task<ApplicationDetail?> GetApplicationDetailAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ApplicationDetail>(
            $"{Base}/api/Applications/{id}");
    }

    public async Task<bool> UpdateApplicationStatusAsync(
        Guid id, string status, string? note = null, int? rating = null)
    {
        var res = await _http.PatchAsJsonAsync(
            $"{Base}/api/Applications/{id}/status",
            new UpdateStatusRequest(status, note, rating));
        return res.IsSuccessStatusCode;
    }

    // ── Interviews ────────────────────────────────
    public async Task<List<InterviewItem>> GetInterviewsAsync(DateTime? date = null)
    {
        var url = $"{Base}/api/Interviews";
        if (date.HasValue)
            url += $"?date={date.Value:yyyy-MM-dd}";
        var result = await _http.GetFromJsonAsync<List<InterviewItem>>(url);
        return result ?? [];
    }

    public async Task<InterviewItem?> CreateInterviewAsync(
        CreateInterviewRequest req)
    {
        var res = await _http.PostAsJsonAsync(
            $"{Base}/api/Interviews", req);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<InterviewItem>();
    }
}