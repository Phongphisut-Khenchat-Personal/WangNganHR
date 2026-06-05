using System.IO;
using System.Text.Json;

namespace WangNganHR.Desktop.Services;

public static class ApiSettings
{
    public const string DefaultBaseUrl = "http://localhost:5083";

    public static string BaseUrl { get; } = LoadBaseUrl();

    private static string LoadBaseUrl()
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path)) return DefaultBaseUrl;

            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            if (doc.RootElement.TryGetProperty("ApiBaseUrl", out var url))
            {
                var value = url.GetString()?.TrimEnd('/');
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
        }
        catch
        {
            // fall back to default
        }

        return DefaultBaseUrl;
    }
}
