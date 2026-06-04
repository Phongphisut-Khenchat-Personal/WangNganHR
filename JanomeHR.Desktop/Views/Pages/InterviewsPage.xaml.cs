using JanomeHR.Desktop.Models;
using JanomeHR.Desktop.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JanomeHR.Desktop.Views.Pages;

public partial class InterviewsPage : Page
{
    private readonly ApiService _api;
    private DateTime _currentDate = DateTime.Today;

    public InterviewsPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
        Loaded += async (_, _) => await LoadAsync();
    }

    private async Task LoadAsync()
    {
        TxtDate.Text = _currentDate.ToString("dddd dd MMMM yyyy",
            new System.Globalization.CultureInfo("th-TH"));

        try
        {
            var items = await _api.GetInterviewsAsync(_currentDate);
            var rows  = items.Select(i => new InterviewRow(i)).ToList();

            LstInterviews.ItemsSource = rows;
            TxtEmpty.Visibility = rows.Any()
                ? Visibility.Collapsed
                : Visibility.Visible;

            TxtTotalToday.Text = rows.Count.ToString();
            TxtOnline.Text     = rows.Count(r => r.Type == "Online").ToString();
            TxtOnsite.Text     = rows.Count(r => r.Type == "Onsite").ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"โหลดข้อมูลไม่ได้: {ex.Message}");
        }
    }

    private async void BtnPrevDay_Click(object s, RoutedEventArgs e)
    {
        _currentDate = _currentDate.AddDays(-1);
        await LoadAsync();
    }

    private async void BtnNextDay_Click(object s, RoutedEventArgs e)
    {
        _currentDate = _currentDate.AddDays(1);
        await LoadAsync();
    }

    private async void BtnToday_Click(object s, RoutedEventArgs e)
    {
        _currentDate = DateTime.Today;
        await LoadAsync();
    }
}

public class InterviewRow
{
    public string TimeText        { get; }
    public string ApplicantName   { get; }
    public string JobPostingTitle { get; }
    public string InterviewerName { get; }
    public string DurationText    { get; }
    public string Type            { get; }
    public string TypeText        { get; }
    public string StatusText      { get; }
    public Brush  TypeColor       { get; }
    public Brush  TypeTextColor   { get; }
    public Brush  StatusColor     { get; }
    public Brush  StatusTextColor { get; }

    public InterviewRow(InterviewItem i)
    {
        TimeText        = i.ScheduledAt.ToLocalTime().ToString("HH:mm");
        ApplicantName   = i.ApplicantName;
        JobPostingTitle = i.JobPostingTitle;
        InterviewerName = i.InterviewerName;
        DurationText    = $"{i.DurationMinutes} นาที";
        Type            = i.Type;

        (TypeText, TypeColor, TypeTextColor) = i.Type switch
        {
            "Online" => ("Online",
                new SolidColorBrush(Color.FromRgb(230,244,236)),
                new SolidColorBrush(Color.FromRgb(26,122,74))),
            _ => ("Onsite",
                new SolidColorBrush(Color.FromRgb(254,243,199)),
                new SolidColorBrush(Color.FromRgb(180,83,9)))
        };

        (StatusText, StatusColor, StatusTextColor) = i.Status switch
        {
            "Scheduled" => ("รอสัมภาษณ์",
                new SolidColorBrush(Color.FromRgb(219,234,254)),
                new SolidColorBrush(Color.FromRgb(29,78,216))),
            "Done" => ("เสร็จแล้ว",
                new SolidColorBrush(Color.FromRgb(230,244,236)),
                new SolidColorBrush(Color.FromRgb(26,122,74))),
            "Cancelled" => ("ยกเลิก",
                new SolidColorBrush(Color.FromRgb(244,244,242)),
                new SolidColorBrush(Color.FromRgb(107,107,107))),
            _ => (i.Status,
                new SolidColorBrush(Colors.LightGray),
                new SolidColorBrush(Colors.Black))
        };
    }
}