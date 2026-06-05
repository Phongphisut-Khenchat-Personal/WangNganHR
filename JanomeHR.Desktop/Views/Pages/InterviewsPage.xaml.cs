using JanomeHR.Desktop.Localization;
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
    private List<InterviewItem> _items = [];

    public InterviewsPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
        Loaded += async (_, _) => await LoadAsync();
        LocalizationService.Instance.LanguageChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        if (IsLoaded) Dispatcher.Invoke(RefreshLocalizedUi);
    }

    private async Task LoadAsync()
    {
        try
        {
            _items = await _api.GetInterviewsAsync(_currentDate);
            RefreshLocalizedUi();
        }
        catch (Exception ex)
        {
            MessageBox.Show(Loc.F("Msg_LoadFailed", ex.Message));
        }
    }

    private void RefreshLocalizedUi()
    {
        UpdateHeaders();
        TxtDate.Text = _currentDate.ToString("dddd dd MMMM yyyy", LocalizationCulture.Current);

        var rows = _items.Select(i => new InterviewRow(i)).ToList();
        LstInterviews.ItemsSource = rows;
        TxtEmpty.Visibility = rows.Any() ? Visibility.Collapsed : Visibility.Visible;

        TxtTotalToday.Text = rows.Count.ToString();
        TxtOnline.Text     = rows.Count(r => r.Type == "Online").ToString();
        TxtOnsite.Text     = rows.Count(r => r.Type == "Onsite").ToString();
    }

    private void UpdateHeaders()
    {
        HdrTime.Text        = Loc.T("Col_Time");
        HdrApplicant.Text   = Loc.T("Col_Applicant");
        HdrJob.Text         = Loc.T("Col_Job");
        HdrInterviewer.Text = Loc.T("Col_Interviewer");
        HdrType.Text        = Loc.T("Col_Type");
        HdrStatus.Text      = Loc.T("Col_Status");
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
        DurationText    = Loc.F("Duration_Minutes", i.DurationMinutes);
        Type            = i.Type;
        TypeText        = StatusLocalizer.InterviewType(i.Type);

        (TypeColor, TypeTextColor) = i.Type switch
        {
            "Online" => (
                new SolidColorBrush(Color.FromRgb(230,244,236)),
                new SolidColorBrush(Color.FromRgb(26,122,74))),
            _ => (
                new SolidColorBrush(Color.FromRgb(254,243,199)),
                new SolidColorBrush(Color.FromRgb(180,83,9)))
        };

        StatusText = StatusLocalizer.InterviewStatus(i.Status);
        (StatusColor, StatusTextColor) = i.Status switch
        {
            "Scheduled" => (
                new SolidColorBrush(Color.FromRgb(219,234,254)),
                new SolidColorBrush(Color.FromRgb(29,78,216))),
            "Done" or "Completed" => (
                new SolidColorBrush(Color.FromRgb(230,244,236)),
                new SolidColorBrush(Color.FromRgb(26,122,74))),
            "Cancelled" => (
                new SolidColorBrush(Color.FromRgb(244,244,242)),
                new SolidColorBrush(Color.FromRgb(107,107,107))),
            _ => (
                new SolidColorBrush(Colors.LightGray),
                new SolidColorBrush(Colors.Black))
        };
    }
}
