namespace WangNganHR.Desktop.Localization;

public static class Loc
{
    public static string T(string key) => LocalizationService.Instance.Get(key);

    public static string F(string key, params object[] args) =>
        LocalizationService.Instance.Format(key, args);
}
