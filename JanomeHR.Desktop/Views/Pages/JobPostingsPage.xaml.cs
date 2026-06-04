using JanomeHR.Desktop.Models;
using JanomeHR.Desktop.Services;
using JanomeHR.Desktop.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JanomeHR.Desktop.Views.Pages;

public partial class JobPostingsPage : Page
{
    private readonly ApiService _api;

    public JobPostingsPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
        Loaded += async (_, _) => await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            var items = await _api.GetJobPostingsAsync();
            LstJobs.ItemsSource = items.Select(j => new JobRow(j)).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"โหลดข้อมูลไม่ได้: {ex.Message}");
        }
    }

    private async void BtnRefresh_Click(object s, RoutedEventArgs e)
        => await LoadAsync();

    private void BtnCreate_Click(object s, RoutedEventArgs e)
    {
        var dlg = new CreateJobPostingWindow(_api);
        dlg.Owner = Window.GetWindow(this);
        if (dlg.ShowDialog() == true)
            _ = LoadAsync();
    }

    private async void BtnPublish_Click(object s, RoutedEventArgs e)
    {
        if (s is not Button btn) return;
        if (!Guid.TryParse(btn.Tag?.ToString(), out var id)) return;

        var ok = await _api.PublishJobPostingAsync(id);
        if (ok) { await LoadAsync(); }
        else MessageBox.Show("เกิดข้อผิดพลาด");
    }

    private async void BtnClose_Click(object s, RoutedEventArgs e)
    {
        if (s is not Button btn) return;
        if (!Guid.TryParse(btn.Tag?.ToString(), out var id)) return;

        var confirm = MessageBox.Show(
            "ยืนยันปิดรับสมัครตำแหน่งนี้?", "ยืนยัน",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        var ok = await _api.CloseJobPostingAsync(id);
        if (ok) await LoadAsync();
        else MessageBox.Show("เกิดข้อผิดพลาด");
    }

    private async void BtnQr_Click(object s, RoutedEventArgs e)
    {
        if (s is not Button btn) return;
        if (!Guid.TryParse(btn.Tag?.ToString(), out var id)) return;

        var qrData = await _api.GenerateQrCodeAsync(id);
        if (qrData is null) { MessageBox.Show("ไม่สามารถสร้าง QR Code ได้"); return; }

        var dlg = new QrCodeWindow(qrData);
        dlg.Owner = Window.GetWindow(this);
        dlg.ShowDialog();
    }
}

public class JobRow
{
    public Guid   Id            { get; }
    public string Title         { get; }
    public string DepartmentName{ get; }
    public string Status        { get; }
    public string StatusText    { get; }
    public string SalaryText    { get; }
    public string Meta          { get; }
    public Brush  StatusColor    { get; }
    public Brush  StatusTextColor{ get; }
    public Visibility PublishVisible { get; }
    public Visibility CloseVisible  { get; }
    public Visibility QrVisible     { get; }

    public JobRow(JobPostingItem j)
    {
        Id             = j.Id;
        Title          = j.Title;
        DepartmentName = j.DepartmentName;
        Status         = j.Status;
        SalaryText     = j.SalaryMin.HasValue
            ? $"฿{j.SalaryMin:N0} – ฿{j.SalaryMax:N0}"
            : "ไม่ระบุเงินเดือน";
        Meta = $"รับ {j.PositionsCount} อัตรา · " +
               $"ผู้สมัคร {j.TotalApplications} คน · " +
               $"สร้างเมื่อ {j.CreatedAt.ToLocalTime():dd/MM/yy}";

        (StatusText, StatusColor, StatusTextColor) = j.Status switch
        {
            "Active" => ("เปิดรับสมัคร",
                new SolidColorBrush(Color.FromRgb(230,244,236)),
                new SolidColorBrush(Color.FromRgb(26,122,74))),
            "Draft"  => ("ฉบับร่าง",
                new SolidColorBrush(Color.FromRgb(254,243,199)),
                new SolidColorBrush(Color.FromRgb(180,83,9))),
            "Closed" => ("ปิดรับแล้ว",
                new SolidColorBrush(Color.FromRgb(244,244,242)),
                new SolidColorBrush(Color.FromRgb(107,107,107))),
            _ => (j.Status,
                new SolidColorBrush(Colors.LightGray),
                new SolidColorBrush(Colors.Black))
        };

        PublishVisible = j.Status == "Draft"   ? Visibility.Visible : Visibility.Collapsed;
        CloseVisible   = j.Status == "Active"  ? Visibility.Visible : Visibility.Collapsed;
        QrVisible      = j.Status == "Active"  ? Visibility.Visible : Visibility.Collapsed;
    }
}