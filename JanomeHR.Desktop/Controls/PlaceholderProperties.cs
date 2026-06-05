using System.Windows;
using System.Windows.Controls;

namespace JanomeHR.Desktop.Controls;

public static class PlaceholderProperties
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.RegisterAttached(
            "Text", typeof(string), typeof(PlaceholderProperties),
            new PropertyMetadata(string.Empty));

    public static string GetText(DependencyObject obj) =>
        (string)obj.GetValue(TextProperty);

    public static void SetText(DependencyObject obj, string value) =>
        obj.SetValue(TextProperty, value);
}
