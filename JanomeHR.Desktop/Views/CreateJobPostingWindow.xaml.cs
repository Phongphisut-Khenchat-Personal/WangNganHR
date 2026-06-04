using JanomeHR.Desktop.Models;
using JanomeHR.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace JanomeHR.Desktop.Views;

public partial class CreateJobPostingWindow : Window
{
    private readonly ApiService _api;

    public CreateJobPostingWindow(ApiService api)
    {
        InitializeComponent();
        _api = api;
        CmbDept.SelectedIndex = 0;
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        TxtError.Text = "";

        if (string.IsNullOrWhiteSpace(TxtTitle.Text) ||
            string.IsNullOrWhiteSpace(TxtDesc.Text) ||
            string.IsNullOrWhiteSpace(TxtReq.Text))
        {
            TxtError.Text = "กรุณากรอกข้อมูลที่จำเป็นให้ครบ";
            return;
        }

        var deptId = int.Parse(
            (CmbDept.SelectedItem as ComboBoxItem)!.Tag!.ToString()!);

        decimal.TryParse(TxtSalMin.Text, out var salMin);
        decimal.TryParse(TxtSalMax.Text, out var salMax);
        int.TryParse(TxtCount.Text, out var count);
        if (count <= 0) count = 1;

        var req = new CreateJobPostingRequest(
            deptId,
            TxtTitle.Text.Trim(),
            TxtDesc.Text.Trim(),
            TxtReq.Text.Trim(),
            salMin > 0 ? salMin : null,
            salMax > 0 ? salMax : null,
            count,
            ChkPublishNow.IsChecked == true ? DateTime.UtcNow : null,
            null);

        try
        {
            var result = await _api.CreateJobPostingAsync(req);
            if (result is not null)
            {
                if (ChkPublishNow.IsChecked == true)
                    await _api.PublishJobPostingAsync(result.Id);

                MessageBox.Show("สร้างประกาศงานเรียบร้อยแล้ว", "สำเร็จ",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else TxtError.Text = "เกิดข้อผิดพลาด กรุณาลองใหม่";
        }
        catch (Exception ex)
        {
            TxtError.Text = $"Error: {ex.Message}";
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
        => Close();
}