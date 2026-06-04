using JanomeHR.Desktop.Services;
using JanomeHR.Desktop.Views.Pages;
using System.Windows;
using System.Windows.Controls;

namespace JanomeHR.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly ApiService _api;

    public MainWindow(ApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        TxtUsername.Text = $"👤 {_api.CurrentUserName} ({_api.CurrentRole})";
        NavigateTo("Applications");
    }

    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
            NavigateTo(btn.Tag?.ToString() ?? "Applications");
    }

    private void NavigateTo(string page)
    {
        TxtPageTitle.Text = page switch
        {
            "Applications" => "ใบสมัครทั้งหมด",
            "Interviews"   => "ปฏิทินสัมภาษณ์",
            "JobPostings"  => "ประกาศงาน",
            _              => page
        };

        SetNavActive(BtnApplications, page == "Applications");
        SetNavActive(BtnInterviews, page == "Interviews");
        SetNavActive(BtnJobPostings, page == "JobPostings");

        Page? content = page switch
        {
            "Applications" => new ApplicationsPage(_api),
            "Interviews"   => new InterviewsPage(_api),
            "JobPostings"  => new JobPostingsPage(_api),
            _              => null
        };

        if (content is not null)
            MainFrame.Navigate(content);
    }

    private static void SetNavActive(Button button, bool active) =>
        button.Style = (Style)Application.Current.FindResource(
            active ? "NavButtonActive" : "NavButton");
}