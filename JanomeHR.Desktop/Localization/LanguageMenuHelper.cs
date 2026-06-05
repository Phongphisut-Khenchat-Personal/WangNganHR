using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace JanomeHR.Desktop.Localization;

public static class LanguageMenuHelper
{
    public static void Populate(ContextMenu menu)
    {
        menu.Items.Clear();
        menu.Items.Add(new MenuItem
        {
            Header = Loc.T("Lang_Label"),
            IsEnabled = false,
            FontWeight = FontWeights.SemiBold
        });
        menu.Items.Add(new Separator());

        var current = LocalizationService.Instance.CurrentCulture;

        foreach (var lang in LocalizationService.Instance.DesktopLanguages)
        {
            var item = new MenuItem
            {
                Header = Loc.T(lang.LabelKey),
                Tag = lang.Code,
                IsCheckable = true,
                IsChecked = lang.Code == current
            };
            item.Click += (_, _) =>
            {
                if (item.Tag is string code)
                    LocalizationService.Instance.SetLanguage(code);
            };
            menu.Items.Add(item);
        }
    }

    public static void Open(Button anchor, ContextMenu menu)
    {
        Populate(menu);
        menu.PlacementTarget = anchor;
        menu.Placement = PlacementMode.Top;
        menu.IsOpen = true;
    }
}
