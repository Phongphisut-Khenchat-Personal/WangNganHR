using WangNganHR.Desktop.Localization;
using WangNganHR.Desktop.Models;
using WangNganHR.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace WangNganHR.Desktop.Views;

public partial class CreateInterviewWindow : Window
{
    private readonly ApiService        _api;
    private readonly ApplicationDetail _applicant;

    public CreateInterviewWindow(ApiService api, ApplicationDetail applicant)
    {
        InitializeComponent();
        _api       = api;
        _applicant = applicant;

        TxtApplicantName.Text =
            $"{applicant.FirstName} {applicant.LastName} — {applicant.JobPostingTitle}";

        DpDate.SelectedDate = DateTime.Today.AddDays(1);

        for (int h = 8; h <= 18; h++)
            CmbHour.Items.Add(new ComboBoxItem { Content = h.ToString("D2"), Tag = h });
        CmbHour.SelectedIndex = 1;

        foreach (var m in new[] { 0, 15, 30, 45 })
            CmbMinute.Items.Add(new ComboBoxItem { Content = m.ToString("D2"), Tag = m });
        CmbMinute.SelectedIndex = 0;

        InitDurationCombo();
        InitTypeCombo();
        UpdateLocationForType();

        LocalizationService.Instance.LanguageChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged() => Dispatcher.Invoke(RefreshLocalizedUi);

    private void RefreshLocalizedUi()
    {
        InitDurationCombo();
        InitTypeCombo();
        UpdateLocationForType();
    }

    private void InitDurationCombo()
    {
        var selected = (CmbDuration.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "30";
        CmbDuration.Items.Clear();
        foreach (var (key, tag) in new[]
        {
            ("Duration_30", "30"), ("Duration_45", "45"), ("Duration_60", "60"),
            ("Duration_90", "90"), ("Duration_120", "120")
        })
            CmbDuration.Items.Add(new ComboBoxItem { Content = Loc.T(key), Tag = tag });

        SelectComboByTag(CmbDuration, selected);
    }

    private void InitTypeCombo()
    {
        var selected = (CmbType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Onsite";
        CmbType.Items.Clear();
        CmbType.Items.Add(new ComboBoxItem { Content = Loc.T("Detail_TypeOnsite"), Tag = "Onsite" });
        CmbType.Items.Add(new ComboBoxItem { Content = Loc.T("Detail_TypeOnline"), Tag = "Online" });
        SelectComboByTag(CmbType, selected);
    }

    private static void SelectComboByTag(ComboBox combo, string tag)
    {
        for (var i = 0; i < combo.Items.Count; i++)
        {
            if (combo.Items[i] is ComboBoxItem item &&
                (item.Tag?.ToString() ?? "") == tag)
            {
                combo.SelectedIndex = i;
                return;
            }
        }
        combo.SelectedIndex = 0;
    }

    private void CmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateLocationForType();

    private void UpdateLocationForType()
    {
        if (LblLocation is null || TxtLocation is null) return;
        var type = (CmbType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Onsite";
        if (type == "Online")
        {
            LblLocation.Text = Loc.T("Detail_LocationOnline");
            if (TxtLocation.Text == Loc.T("Detail_LocationDefault") || string.IsNullOrWhiteSpace(TxtLocation.Text))
                TxtLocation.Text = Loc.T("Detail_MeetingUrlDefault");
        }
        else
        {
            LblLocation.Text = Loc.T("Detail_Location");
            if (TxtLocation.Text.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                TxtLocation.Text = Loc.T("Detail_LocationDefault");
        }
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        TxtError.Visibility = Visibility.Collapsed;

        if (DpDate.SelectedDate is null)
        {
            ShowError(Loc.T("CreateInterview_SelectDate"));
            return;
        }

        var hour   = int.Parse((CmbHour.SelectedItem   as ComboBoxItem)!.Tag!.ToString()!);
        var minute = int.Parse((CmbMinute.SelectedItem as ComboBoxItem)!.Tag!.ToString()!);
        var dur    = int.Parse((CmbDuration.SelectedItem as ComboBoxItem)!.Tag!.ToString()!);
        var type   = (CmbType.SelectedItem as ComboBoxItem)!.Tag!.ToString()!;

        var scheduledAt = DpDate.SelectedDate.Value.Date
            .AddHours(hour).AddMinutes(minute);

        if (_api.CurrentUserId is null)
        {
            ShowError(Loc.T("Msg_NoUser"));
            return;
        }

        var req = new CreateInterviewRequest(
            _applicant.Id,
            _api.CurrentUserId.Value,
            scheduledAt,
            dur,
            type,
            TxtLocation.Text.Trim());

        try
        {
            var result = await _api.CreateInterviewAsync(req);
            if (result is not null)
            {
                MessageBox.Show(
                    Loc.F("CreateInterview_Success", Environment.NewLine, scheduledAt, type),
                    Loc.T("Msg_SuccessTitle"), MessageBoxButton.OK,
                    MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else ShowError(Loc.T("Msg_Retry"));
        }
        catch (Exception ex)
        {
            ShowError(Loc.F("Msg_Error", ex.Message));
        }
    }

    private void ShowError(string msg)
    {
        TxtError.Text       = msg;
        TxtError.Visibility = Visibility.Visible;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
        => Close();
}
