using System.Windows;

namespace WangNganHR.Desktop.Localization;

public sealed class LocalizationService
{
    public static LocalizationService Instance { get; } = new();

    private ResourceDictionary? _strings;
    private string _culture = "en";

    public event Action? LanguageChanged;

    public IReadOnlyList<LanguageOption> DesktopLanguages { get; } =
    [
        new("th", "Lang_Thai"),
        new("en", "Lang_English"),
        new("ja", "Lang_Japanese")
    ];

    public string CurrentCulture => _culture;

    public void Initialize(string culture = "en") => SetLanguage(culture);

    public void SetLanguage(string culture)
    {
        culture = culture.ToLowerInvariant();
        if (culture is not ("th" or "en" or "ja"))
            culture = "en";

        if (_culture == culture && _strings is not null)
            return;

        if (_strings is not null)
            Application.Current.Resources.MergedDictionaries.Remove(_strings);

        _strings = new ResourceDictionary
        {
            Source = new Uri(
                $"pack://application:,,,/WangNganHR.Desktop;component/Resources/Strings/Strings.{culture}.xaml",
                UriKind.Absolute)
        };
        Application.Current.Resources.MergedDictionaries.Add(_strings);
        _culture = culture;
        LanguageChanged?.Invoke();
    }

    public string Get(string key)
    {
        if (Application.Current.TryFindResource(key) is string s)
            return s;
        return key;
    }

    public string Format(string key, params object[] args) =>
        string.Format(Get(key), args);

    public record LanguageOption(string Code, string LabelKey);
}
