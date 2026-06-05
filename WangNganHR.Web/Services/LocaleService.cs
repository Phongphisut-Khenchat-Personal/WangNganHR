using System.Globalization;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace WangNganHR.Web.Services;

public sealed class LocaleService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    private Dictionary<string, string> _strings = new(StringComparer.OrdinalIgnoreCase);

    public event Action? Changed;

    public string Culture { get; private set; } = "en";

    public IReadOnlyList<LanguageOption> Languages { get; } =
    [
        new("th", "Lang_Thai"),
        new("en", "Lang_English"),
        new("my", "Lang_Myanmar"),
        new("ja", "Lang_Japanese"),
        new("lo", "Lang_Lao")
    ];

    public LocaleService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task InitializeAsync()
    {
        var saved = await _js.InvokeAsync<string?>("localStorage.getItem", "janomehr.lang");
        var culture = string.IsNullOrWhiteSpace(saved) ? "en" : saved!;
        await SetCultureAsync(culture);
    }

    public async Task SetCultureAsync(string culture)
    {
        culture = culture.ToLowerInvariant();
        if (!Languages.Any(l => l.Code == culture))
            culture = "en";

        var data = await _http.GetFromJsonAsync<Dictionary<string, string>>($"locales/{culture}.json");
        _strings = data ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Culture = culture;
        await _js.InvokeVoidAsync("localStorage.setItem", "janomehr.lang", culture);
        try
        {
            await _js.InvokeVoidAsync("janomeHr.setLang", culture);
            await _js.InvokeVoidAsync("janomeHr.applyErrorUi", new
            {
                message = T("Error_Message"),
                reload = T("Error_Reload"),
                dismiss = T("Error_Dismiss")
            });
        }
        catch { /* static render */ }
        Changed?.Invoke();
    }

    public CultureInfo GetCulture() => Culture switch
    {
        "th" => new CultureInfo("th-TH"),
        "ja" => new CultureInfo("ja-JP"),
        "my" => new CultureInfo("my-MM"),
        "lo" => new CultureInfo("lo-LA"),
        _    => new CultureInfo("en-US")
    };

    public string T(string key) =>
        _strings.TryGetValue(key, out var value) ? value : key;

    public string F(string key, params object[] args) =>
        string.Format(T(key), args);

    public record LanguageOption(string Code, string LabelKey);
}
