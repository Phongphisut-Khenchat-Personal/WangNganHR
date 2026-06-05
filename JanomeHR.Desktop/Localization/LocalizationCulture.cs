using System.Globalization;

namespace JanomeHR.Desktop.Localization;

public static class LocalizationCulture
{
    public static CultureInfo Current =>
        LocalizationService.Instance.CurrentCulture switch
        {
            "en" => new CultureInfo("en-US"),
            "ja" => new CultureInfo("ja-JP"),
            _ => new CultureInfo("th-TH")
        };
}
