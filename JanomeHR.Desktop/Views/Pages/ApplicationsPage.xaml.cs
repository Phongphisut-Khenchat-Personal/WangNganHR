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
    private List<ApplicationRow> _allRows = [];

    public ApplicationsPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
        CmbStatus.SelectedIndex = 0;
        Loaded += async (_, _) => await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            var items = await _api.GetApplicationsAsync();
            _allRows  = items.Select(a => new ApplicationRow(a)).ToList();

            UpdateStats();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"โหลดข้อมูลไม่ได้: {ex.Message}");
        }
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

        var filtered = _allRows
            .Where(r => string.IsNullOrEmpty(search) ||
                        r.FullName.ToLower().Contains(search) ||
                        r.ReferenceCode.ToLower().Contains(search) ||
                        r.Phone.Contains(search))
            .Where(r => string.IsNullOrEmpty(status) || r.Status == status)
            .ToList();

        DgApplications.ItemsSource = filtered;
        TxtCount.Text = $"แสดง {filtered.Count} รายการ จากทั้งหมด {_allRows.Count} รายการ";
    }

    private void TxtSearch_TextChanged(object s, TextChangedEventArgs e)
        => ApplyFilter();

    private void CmbStatus_SelectionChanged(object s, SelectionChangedEventArgs e)
        => ApplyFilter();

    private async void BtnRefresh_Click(object s, RoutedEventArgs e)
        => await LoadAsync();

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
            MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}");
        }
    }
}

// ── View Model Row ────────────────────────────────
public class ApplicationRow
{
    public Guid   Id             { get; }
    public string ReferenceCode  { get; }
    public string FullName       { get; }
    public string JobPostingTitle{ get; }
    public string Phone          { get; }
    public string Status         { get; }
    public string SourceText     { get; }
    public string CreatedAtText  { get; }
    public string StatusText     { get; }
    public Brush  StatusColor    { get; }
    public Brush  StatusTextColor{ get; }

    public ApplicationRow(ApplicationItem a)
    {
        Id              = a.Id;
        ReferenceCode   = a.ReferenceCode;
        FullName        = $"{a.FirstName} {a.LastName}";
        JobPostingTitle = a.JobPostingTitle;
        Phone           = a.Phone;
        Status          = a.Status;
        CreatedAtText   = a.CreatedAt.ToLocalTime().ToString("dd/MM/yy HH:mm");
        SourceText      = a.Source switch
        {
            "QRCode"  => "QR Code",
            "WalkIn"  => "เดินมาเอง",
            "Web"     => "Web",
            _         => a.Source
        };
        (StatusText, StatusColor, StatusTextColor) = a.Status switch
        {
            "New"       => ("ใหม่",
                new SolidColorBrush(Color.FromRgb(252,232,236)),
                new SolidColorBrush(Color.FromRgb(158,11,35))),
            "Review"    => ("กำลังพิจารณา",
                new SolidColorBrush(Color.FromRgb(254,243,199)),
                new SolidColorBrush(Color.FromRgb(180,83,9))),
            "Interview" => ("นัดสัมภาษณ์",
                new SolidColorBrush(Color.FromRgb(219,234,254)),
                new SolidColorBrush(Color.FromRgb(29,78,216))),
            "Pass"      => ("ผ่าน",
                new SolidColorBrush(Color.FromRgb(230,244,236)),
                new SolidColorBrush(Color.FromRgb(26,122,74))),
            "Fail"      => ("ไม่ผ่าน",
                new SolidColorBrush(Color.FromRgb(244,244,242)),
                new SolidColorBrush(Color.FromRgb(107,107,107))),
            _ => (a.Status,
                new SolidColorBrush(Colors.LightGray),
                new SolidColorBrush(Colors.Black))
        };
    }
}