using System.Windows;
using System.Windows.Controls;

namespace WangNganHR.Desktop.Controls;

public partial class BulletListInput : UserControl
{
    private readonly List<TextBox> _boxes = [];

    public BulletListInput()
    {
        InitializeComponent();
        AddRow();
    }

    public void SetCaption(string text) => LblCaption.Text = text;

    public void SetAddLabel(string text) => BtnAddContent.Label = text;

    public void SetPlaceholder(string text)
    {
        foreach (var box in _boxes)
            PlaceholderProperties.SetText(box, text);
    }

    public void SetItems(IEnumerable<string> items)
    {
        ItemsPanel.Children.Clear();
        _boxes.Clear();

        var list = items.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        if (list.Count == 0)
            AddRow();
        else
            foreach (var item in list)
                AddRow(item);
    }

    public List<string> GetItems() =>
        _boxes.Select(b => b.Text.Trim())
              .Where(x => !string.IsNullOrWhiteSpace(x))
              .ToList();

    private void BtnAdd_Click(object sender, RoutedEventArgs e) => AddRow();

    private void AddRow(string? value = null)
    {
        var grid = new Grid { Margin = new Thickness(0, 0, 0, 8) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var box = new TextBox
        {
            Text = value ?? "",
            BorderBrush = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E0DEDD")!),
            Background = System.Windows.Media.Brushes.White,
            MinHeight = 34,
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = false,
            VerticalContentAlignment = VerticalAlignment.Center
        };

        var remove = new Button
        {
            Margin = new Thickness(8, 0, 0, 0),
            Padding = new Thickness(8, 6, 8, 6),
            Background = System.Windows.Media.Brushes.Transparent,
            BorderBrush = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E0DEDD")!),
            BorderThickness = new Thickness(1),
            Cursor = System.Windows.Input.Cursors.Hand,
            VerticalAlignment = VerticalAlignment.Center,
            Content = new FaIcon
            {
                Icon = "xmark",
                IconBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#6B6B6B")!),
                Width = 14,
                Height = 14
            }
        };

        remove.Click += (_, _) => RemoveRow(grid, box);
        Grid.SetColumn(box, 0);
        Grid.SetColumn(remove, 1);
        grid.Children.Add(box);
        grid.Children.Add(remove);

        _boxes.Add(box);
        ItemsPanel.Children.Add(grid);
    }

    private void RemoveRow(Grid row, TextBox box)
    {
        if (_boxes.Count <= 1)
        {
            box.Clear();
            return;
        }

        _boxes.Remove(box);
        ItemsPanel.Children.Remove(row);
    }
}
