using JanomeHR.Desktop.Localization;
using JanomeHR.Desktop.Models;
using JanomeHR.Desktop.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace JanomeHR.Desktop.Views.Pages;

public partial class ApplicationsPage : Page
{
    private readonly ApiService _api;
    private List<ApplicationItem> _items = [];
    private List<ApplicationRow> _allRows = [];
    private bool _filterComboReady;

    public ApplicationsPage(ApiService api)
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
            _items = await _api.GetApplicationsAsync();
            RebuildRows();
            InitFilterCombos();
            UpdateGridHeaders();
            UpdateStats();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            MessageBox.Show(Loc.F("Msg_LoadFailed", ex.Message));
        }
    }

    private void RebuildRows() =>
        _allRows = _items.Select(a => new ApplicationRow(a)).ToList();

    private void RefreshLocalizedUi()
    {
        RebuildRows();
        InitFilterCombos();
        UpdateGridHeaders();
        UpdateStats();
        ApplyFilter();
    }

    private void InitFilterCombos()
    {
        var statusSel = (CmbStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";
        var jobSel = (CmbJob.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";
        var dateSel = (CmbDate.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";

        _filterComboReady = false;

        CmbJob.Items.Clear();
        CmbJob.Items.Add(new ComboBoxItem { Content = Loc.T("Filter_AllJobs"), Tag = "" });
        foreach (var title in _items.Select(a => a.JobPostingTitle).Distinct().OrderBy(t => t))
            CmbJob.Items.Add(new ComboBoxItem { Content = title, Tag = title });

        CmbStatus.Items.Clear();
        CmbStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Filter_AllStatus"), Tag = "" });
        CmbStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Status_New"), Tag = "New" });
        CmbStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Status_Review"), Tag = "Review" });
        CmbStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Status_Interview"), Tag = "Interview" });
        CmbStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Status_Pass"), Tag = "Pass" });
        CmbStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Status_Fail"), Tag = "Fail" });

        CmbDate.Items.Clear();
        CmbDate.Items.Add(new ComboBoxItem { Content = Loc.T("Filter_AllDates"), Tag = "" });
        CmbDate.Items.Add(new ComboBoxItem { Content = Loc.T("Filter_Today"), Tag = "0" });
        CmbDate.Items.Add(new ComboBoxItem { Content = Loc.T("Filter_7Days"), Tag = "7" });
        CmbDate.Items.Add(new ComboBoxItem { Content = Loc.T("Filter_30Days"), Tag = "30" });

        SelectComboTag(CmbJob, jobSel);
        SelectComboTag(CmbStatus, statusSel);
        SelectComboTag(CmbDate, dateSel);

        _filterComboReady = true;
    }

    private static void SelectComboTag(ComboBox combo, string tag)
    {
        for (var i = 0; i < combo.Items.Count; i++)
        {
            if (combo.Items[i] is ComboBoxItem item && (item.Tag?.ToString() ?? "") == tag)
            {
                combo.SelectedIndex = i;
                return;
            }
        }
        combo.SelectedIndex = 0;
    }

    private void UpdateGridHeaders()
    {
        ColReference.Header = Loc.T("Col_Reference");
        ColFullName.Header = Loc.T("Col_FullName");
        ColJob.Header = Loc.T("Col_Job");
        ColPhone.Header = Loc.T("Col_Phone");
        ColSource.Header = Loc.T("Col_Source");
        ColStatus.Header = Loc.T("Col_Status");
        ColCreatedAt.Header = Loc.T("Col_CreatedAt");
    }

    private void UpdateStats()
    {
        TxtTotalNew.Text       = _allRows.Count(r => r.Status == "New").ToString();
        TxtTotalReview.Text    = _allRows.Count(r => r.Status == "Review").ToString();
        TxtTotalInterview.Text = _allRows.Count(r => r.Status == "Interview").ToString();
        TxtTotalPass.Text      = _allRows.Count(r => r.Status == "Pass").ToString();
    }

    private void ApplyFilter()
    {
        var search = TxtSearch.Text.ToLower();
        var status = (CmbStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";
        var job = (CmbJob.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";
        var dateTag = (CmbDate.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";
        DateTime? since = dateTag switch
        {
            "0" => DateTime.Today,
            "7" => DateTime.Today.AddDays(-7),
            "30" => DateTime.Today.AddDays(-30),
            _ => null
        };

        var filtered = _allRows
            .Where(r => string.IsNullOrEmpty(search) ||
                        r.FullName.ToLower().Contains(search) ||
                        r.ReferenceCode.ToLower().Contains(search) ||
                        r.Phone.Contains(search))
            .Where(r => string.IsNullOrEmpty(status) || r.Status == status)
            .Where(r => string.IsNullOrEmpty(job) || r.JobPostingTitle == job)
            .Where(r => since is null || r.CreatedAt >= since.Value)
            .ToList();

        DgApplications.ItemsSource = filtered;
        TxtCount.Text = Loc.F("Count_Format", filtered.Count, _allRows.Count);
    }

    private void TxtSearch_TextChanged(object s, TextChangedEventArgs e) => ApplyFilter();

    private void CmbFilter_Changed(object s, SelectionChangedEventArgs e)
    {
        if (_filterComboReady)
            ApplyFilter();
    }

    private async void BtnRefresh_Click(object s, RoutedEventArgs e) => await LoadAsync();

    private async void DgApplications_DoubleClick(object s, MouseButtonEventArgs e)
    {
        if (DgApplications.SelectedItem is not ApplicationRow row) return;

        try
        {
            var detail = await _api.GetApplicationDetailAsync(row.Id);
            if (detail is null) return;

            var dlg = new ApplicationDetailWindow(detail, _api);
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() == true)
                await LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(Loc.F("Msg_Error", ex.Message));
        }
    }
}

public class ApplicationRow
{
    public Guid   Id              { get; }
    public string ReferenceCode   { get; }
    public string FullName        { get; }
    public string JobPostingTitle { get; }
    public string Phone           { get; }
    public string Status          { get; }
    public string SourceText      { get; }
    public string CreatedAtDate   { get; }
    public string CreatedAtTime   { get; }
    public DateTime CreatedAt     { get; }
    public string StatusText      { get; }
    public Brush  StatusColor     { get; }
    public Brush  StatusTextColor { get; }

    public ApplicationRow(ApplicationItem a)
    {
        Id              = a.Id;
        ReferenceCode   = a.ReferenceCode;
        FullName        = $"{a.FirstName} {a.LastName}";
        JobPostingTitle = a.JobPostingTitle;
        Phone           = a.Phone;
        Status          = a.Status;
        var localCreated = a.CreatedAt.ToLocalTime();
        CreatedAt       = localCreated.Date;
        CreatedAtDate   = localCreated.ToString("dd/MM/yy");
        CreatedAtTime   = localCreated.ToString("HH:mm");
        SourceText      = StatusLocalizer.ApplicationSource(a.Source);
        (StatusText, StatusColor, StatusTextColor) = a.Status switch
        {
            "New" => (StatusLocalizer.ApplicationStatus("New"),
                new SolidColorBrush(Color.FromRgb(252,232,236)),
                new SolidColorBrush(Color.FromRgb(158,11,35))),
            "Review" => (StatusLocalizer.ApplicationStatus("Review"),
                new SolidColorBrush(Color.FromRgb(254,243,199)),
                new SolidColorBrush(Color.FromRgb(180,83,9))),
            "Interview" => (StatusLocalizer.ApplicationStatus("Interview"),
                new SolidColorBrush(Color.FromRgb(219,234,254)),
                new SolidColorBrush(Color.FromRgb(29,78,216))),
            "Pass" => (StatusLocalizer.ApplicationStatus("Pass"),
                new SolidColorBrush(Color.FromRgb(230,244,236)),
                new SolidColorBrush(Color.FromRgb(26,122,74))),
            "Fail" => (StatusLocalizer.ApplicationStatus("Fail"),
                new SolidColorBrush(Color.FromRgb(244,244,242)),
                new SolidColorBrush(Color.FromRgb(107,107,107))),
            _ => (StatusLocalizer.ApplicationStatus(a.Status),
                new SolidColorBrush(Colors.LightGray),
                new SolidColorBrush(Colors.Black))
        };
    }
}
