using JanomeHR.Desktop.Models;
using JanomeHR.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace JanomeHR.Desktop.Views;

public partial class ApplicationDetailWindow : Window
{
    private readonly ApplicationDetail _detail;
    private readonly ApiService _api;

    public ApplicationDetailWindow(ApplicationDetail detail, ApiService api)
    {
        InitializeComponent();
        _detail = detail;
        _api    = api;
        LoadDetail();
    }

    private void LoadDetail()
    {
        TxtRefCode.Text    = _detail.ReferenceCode;
        TxtName.Text       = $"{_detail.FirstName} {_detail.LastName}";
        TxtPosition.Text   = $"{_detail.JobPostingTitle} · {_detail.DepartmentName}";
        TxtPhone.Text      = _detail.Phone;
        TxtEmail.Text      = _detail.Email ?? "–";
        TxtEducation.Text  = _detail.EducationLevel;
        TxtExperience.Text = $"{_detail.ExperienceYears} ปี";
        TxtSalary.Text     = _detail.SalaryExpected.HasValue
            ? $"฿{_detail.SalaryExpected.Value:N0}"
            : "–";
        TxtAvailable.Text  = _detail.AvailableDate?.ToString("dd/MM/yyyy") ?? "–";
        TxtStatus.Text     = _detail.Status;
        TxtSource.Text     = _detail.Source;

        // set current status in combobox
        foreach (ComboBoxItem item in CmbNewStatus.Items)
            if (item.Tag?.ToString() == _detail.Status)
            { CmbNewStatus.SelectedItem = item; break; }

        // notes
        LstNotes.ItemsSource = _detail.Notes.Select(n => new
        {
            n.Content,
            n.CreatedByName,
            CreatedAtText = n.CreatedAt.ToLocalTime().ToString("dd/MM/yy HH:mm")
        }).ToList();
    }

    private async void BtnSaveStatus_Click(object sender, RoutedEventArgs e)
    {
        var status = (CmbNewStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString();
        if (string.IsNullOrEmpty(status)) return;

        var ok = await _api.UpdateApplicationStatusAsync(
            _detail.Id, status,
            string.IsNullOrWhiteSpace(TxtNote.Text) ? null : TxtNote.Text.Trim());

        if (ok)
        {
            MessageBox.Show("บันทึกสถานะเรียบร้อยแล้ว", "สำเร็จ",
                MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        else
            MessageBox.Show("เกิดข้อผิดพลาด กรุณาลองใหม่", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
        => Close();
}