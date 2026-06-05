using WangNganHR.Desktop.Localization;

using WangNganHR.Desktop.Services;

using WangNganHR.Desktop.Views.Pages;

using System.Windows;

using System.Windows.Controls;



namespace WangNganHR.Desktop.Views;



public partial class MainWindow : Window

{

    private readonly ApiService _api;

    private string _currentPage = "Applications";



    public MainWindow(ApiService api)

    {

        InitializeComponent();

        _api = api;

        LocalizationService.Instance.LanguageChanged += OnLanguageChanged;

    }



    protected override void OnContentRendered(EventArgs e)

    {

        base.OnContentRendered(e);

        TxtUsername.Text = $"{_api.CurrentUserName} ({_api.CurrentRole})";

        NavigateTo("Applications");

    }



    private void BtnSettings_Click(object sender, RoutedEventArgs e) =>

        LanguageMenuHelper.Open(BtnSettings, LanguageMenu);



    private void OnLanguageChanged() => NavigateTo(_currentPage);



    private void NavButton_Click(object sender, RoutedEventArgs e)

    {

        if (sender is Button btn)

            NavigateTo(btn.Tag?.ToString() ?? "Applications");

    }



    private void NavigateTo(string page)

    {

        _currentPage = page;



        TxtPageTitle.Text = page switch

        {

            "Applications" => Loc.T("Page_Applications"),

            "Interviews"   => Loc.T("Page_Interviews"),

            "JobPostings"  => Loc.T("Page_JobPostings"),

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

