using WangNganHR.Desktop.Localization;
using WangNganHR.Desktop.Models;
using WangNganHR.Desktop.Services;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WangNganHR.Desktop.Views;

public partial class ApplicationDetailWindow : Window
{
    private const string ApiBase = "http://localhost:5083";

    private readonly ApplicationDetail _detail;
    private readonly ApiService _api;
    private bool _statusComboReady;
    private int _step;

    public ApplicationDetailWindow(ApplicationDetail detail, ApiService api)
    {
        InitializeComponent();
        _detail = detail;
        _api = api;
        InitInterviewForm();
        LoadPreview();
        InitStatusCombo();
        SetStep(0);
        LocalizationService.Instance.LanguageChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged() => Dispatcher.Invoke(RefreshLocalizedUi);

    private void RefreshLocalizedUi()
    {
        LoadPreview();
        InitStatusCombo();
        InitDurationCombo();
        InitInterviewTypeCombo();
        UpdateLocationForType();
        SetStep(_step);
    }

    private void LoadPreview()
    {
        TxtRefCode.Text = _detail.ReferenceCode;
        TxtName.Text = FormatName();
        TxtPosition.Text = _detail.JobPostingTitle;
        TxtDepartment.Text = _detail.DepartmentName;
        TxtDepartment.Visibility = string.IsNullOrWhiteSpace(_detail.DepartmentName)
            ? Visibility.Collapsed : Visibility.Visible;
        TxtStatusBadge.Text = StatusLocalizer.ApplicationStatus(_detail.Status);
        ApplyStatusBadgeColor(_detail.Status);

        PreviewSections.ItemsSource = BuildPreviewSections();

        if (_detail.IsFreshGraduate || _detail.WorkExperiences.Count == 0)
        {
            BorderWorkHistory.Visibility = Visibility.Collapsed;
        }
        else
        {
            BorderWorkHistory.Visibility = Visibility.Visible;
            PreviewWorkHistory.ItemsSource = _detail.WorkExperiences.Select((w, i) => new
            {
                Title = Loc.F("Detail_WorkItem", i + 1) + ": " + w.CompanyName,
                Subtitle = w.Position + FormatPeriod(w.StartDate, w.EndDate),
                Details = BuildWorkDetails(w)
            }).ToList();
        }

        if (_detail.Documents.Count > 0)
        {
            PnlDocuments.Visibility = Visibility.Visible;
            PreviewDocuments.ItemsSource = _detail.Documents.Select(d => new
            {
                d.FileName,
                TypeLabel = StatusLocalizer.DocumentType(d.DocumentType),
                Url = ApiBase + d.FileUrl
            }).ToList();
        }
        else
        {
            PnlDocuments.Visibility = Visibility.Collapsed;
        }

        var notes = _detail.Notes.Select(n => new
        {
            n.Content, n.CreatedByName,
            CreatedAtText = n.CreatedAt.ToLocalTime().ToString("dd/MM/yy HH:mm")
        }).ToList();
        LstNotes.ItemsSource = notes;
        TxtNoNotes.Visibility = notes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private string FormatName()
    {
        var prefix = string.IsNullOrWhiteSpace(_detail.TitlePrefix)
            ? "" : StatusLocalizer.TitlePrefix(_detail.TitlePrefix) + " ";
        return $"{prefix}{_detail.FirstName} {_detail.LastName}".Trim();
    }

    private List<PreviewSection> BuildPreviewSections()
    {
        var sections = new List<PreviewSection>();

        sections.Add(new PreviewSection(Loc.T("Detail_Section_Personal"), [
            Field(Loc.T("Detail_Phone"), _detail.Phone),
            Field(Loc.T("Detail_Email"), _detail.Email),
            Field(Loc.T("Detail_NationalId"), _detail.NationalId),
            Field(Loc.T("Detail_Birthdate"), FmtDate(_detail.Birthdate)),
            Field(Loc.T("Detail_Gender"), StatusLocalizer.Gender(_detail.Gender)),
            Field(Loc.T("Detail_Nationality"), StatusLocalizer.Nationality(_detail.NationalityType)),
            Field(Loc.T("Detail_LineId"), _detail.LineId),
            Field(Loc.T("Detail_Address"), _detail.Address),
            Field(Loc.T("Detail_RegisteredAddress"), _detail.SameAsCurrentAddress
                ? Loc.T("Detail_SameAsCurrent") : _detail.RegisteredAddress),
            Field(Loc.T("Detail_Province"), JoinAddressParts()),
            Field(Loc.T("Detail_EmergencyName"), _detail.EmergencyContactName),
            Field(Loc.T("Detail_EmergencyPhone"), _detail.EmergencyContactPhone),
            Field(Loc.T("Detail_EmergencyRelation"), _detail.EmergencyContactRelation),
        ]));

        sections.Add(new PreviewSection(Loc.T("Detail_Section_Education"), [
            Field(Loc.T("Detail_Education"), StatusLocalizer.EducationLevel(_detail.EducationLevel)),
            Field(Loc.T("Detail_Field"), _detail.EducationField),
            Field(Loc.T("Detail_School"), _detail.SchoolName),
            Field(Loc.T("Detail_GraduationYear"), _detail.GraduationYear?.ToString()),
            Field(Loc.T("Detail_Gpa"), _detail.Gpa?.ToString("0.00")),
            Field(Loc.T("Detail_SkillThai"), StatusLocalizer.SkillLevel(_detail.SkillThai)),
            Field(Loc.T("Detail_SkillEnglish"), StatusLocalizer.SkillLevel(_detail.SkillEnglish)),
            Field(Loc.T("Detail_SkillJapanese"), StatusLocalizer.SkillLevel(_detail.SkillJapanese)),
            Field(Loc.T("Detail_ComputerSkills"), _detail.ComputerSkills),
            Field(Loc.T("Detail_DriversLicense"), FmtBool(_detail.HasDriversLicense)),
            Field(Loc.T("Detail_OwnVehicle"), FmtBool(_detail.HasOwnVehicle)),
        ]));

        sections.Add(new PreviewSection(Loc.T("Detail_Section_Work"), [
            Field(Loc.T("Detail_Experience"), Loc.F("Years_Format", _detail.ExperienceYears)),
            Field(Loc.T("Detail_FreshGraduate"), _detail.IsFreshGraduate ? Loc.T("Yes") : Loc.T("No")),
            Field(Loc.T("Detail_Salary"), FmtSalary(_detail.SalaryExpected)),
            Field(Loc.T("Detail_LastSalary"), FmtSalary(_detail.LastSalary)),
            Field(Loc.T("Detail_Available"), FmtDate(_detail.AvailableDate)),
            Field(Loc.T("Detail_ShiftWork"), FmtBool(_detail.WillingShiftWork)),
            Field(Loc.T("Detail_Overtime"), FmtBool(_detail.WillingOvertime)),
            Field(Loc.T("Detail_Relocate"), FmtBool(_detail.WillingRelocate)),
            Field(Loc.T("Detail_Source"), StatusLocalizer.ApplicationSource(_detail.Source)),
            Field(Loc.T("Detail_ReferralSource"), StatusLocalizer.ReferralSource(_detail.ReferralSource)),
            Field(Loc.T("Detail_PdpaConsented"), FmtPdpa(_detail.PdpaConsentedAt)),
            Field(Loc.T("Detail_AppliedAt"), _detail.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm")),
        ]));

        return sections;
    }

    private PreviewField Field(string label, string? value) =>
        new(label, string.IsNullOrWhiteSpace(value) ? "–" : value);

    private string? JoinAddressParts()
    {
        var parts = new[] { _detail.District, _detail.Province, _detail.PostalCode }
            .Where(p => !string.IsNullOrWhiteSpace(p));
        var joined = string.Join(", ", parts);
        return string.IsNullOrWhiteSpace(joined) ? null : joined;
    }

    private static string FmtDate(DateOnly? d) => d?.ToString("dd/MM/yyyy") ?? "–";

    private string FmtSalary(decimal? v) =>
        v.HasValue ? Loc.F("Salary_Format", v.Value) : "–";

    private string FmtBool(bool? v) => v switch
    {
        true => Loc.T("Yes"),
        false => Loc.T("No"),
        _ => "–"
    };

    private string FmtPdpa(DateTime? consentedAt) =>
        consentedAt is null
            ? "–"
            : Loc.F("Detail_Pdpa_Yes", consentedAt.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm"));

    private static string FormatPeriod(DateOnly? start, DateOnly? end)
    {
        if (start is null && end is null) return "";
        return $" ({start?.ToString("dd/MM/yy") ?? "?"} – {end?.ToString("dd/MM/yy") ?? "?"})";
    }

    private string BuildWorkDetails(WorkExperienceItem w)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(w.Responsibilities))
            parts.Add(w.Responsibilities);
        if (!string.IsNullOrWhiteSpace(w.ReasonForLeaving))
            parts.Add(Loc.T("Detail_LeaveReason") + ": " + w.ReasonForLeaving);
        if (w.Salary.HasValue)
            parts.Add(Loc.F("Salary_Format", w.Salary.Value));
        return parts.Count > 0 ? string.Join(Environment.NewLine, parts) : "–";
    }

    private void ApplyStatusBadgeColor(string status)
    {
        var (bg, fg) = status switch
        {
            "New" => ("#DBEAFE", "#1D4ED8"),
            "Review" => ("#FEF3C7", "#92400E"),
            "Interview" => ("#EDE9FE", "#5B21B6"),
            "Pass" => ("#D1FAE5", "#065F46"),
            "Fail" => ("#FEE2E2", "#991B1B"),
            _ => ("#374151", "#FFFFFF")
        };
        BadgeStatus.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bg)!);
        TxtStatusBadge.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fg)!);
    }

    private void SetStep(int step)
    {
        _step = step;
        var isPreview = step == 0;

        PnlPreview.Visibility = isPreview ? Visibility.Visible : Visibility.Collapsed;
        PnlUpdate.Visibility = isPreview ? Visibility.Collapsed : Visibility.Visible;

        BtnStepPreview.Style = (Style)FindResource(isPreview ? "DetailStepBtnActive" : "DetailStepBtn");
        BtnStepUpdate.Style = (Style)FindResource(isPreview ? "DetailStepBtn" : "DetailStepBtnActive");

        BtnNext.Visibility = isPreview ? Visibility.Visible : Visibility.Collapsed;
        BtnBack.Visibility = isPreview ? Visibility.Collapsed : Visibility.Visible;
        BtnSave.Visibility = isPreview ? Visibility.Collapsed : Visibility.Visible;
    }

    private void BtnStepPreview_Click(object sender, RoutedEventArgs e) => SetStep(0);
    private void BtnStepUpdate_Click(object sender, RoutedEventArgs e) => SetStep(1);
    private void BtnNext_Click(object sender, RoutedEventArgs e) => SetStep(1);
    private void BtnBack_Click(object sender, RoutedEventArgs e) => SetStep(0);

    private void BtnOpenDocument_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string url })
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    private void InitStatusCombo()
    {
        var selected = (CmbNewStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString()
                       ?? _detail.Status;

        _statusComboReady = false;
        CmbNewStatus.Items.Clear();
        CmbNewStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Status_New"), Tag = "New" });
        CmbNewStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Status_Review"), Tag = "Review" });
        CmbNewStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Status_Interview"), Tag = "Interview" });
        CmbNewStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Status_PassFull"), Tag = "Pass" });
        CmbNewStatus.Items.Add(new ComboBoxItem { Content = Loc.T("Status_Fail"), Tag = "Fail" });

        for (var i = 0; i < CmbNewStatus.Items.Count; i++)
        {
            if (CmbNewStatus.Items[i] is ComboBoxItem item &&
                (item.Tag?.ToString() ?? "") == selected)
            {
                CmbNewStatus.SelectedIndex = i;
                break;
            }
        }

        if (CmbNewStatus.SelectedIndex < 0)
            CmbNewStatus.SelectedIndex = 0;

        _statusComboReady = true;
        if (PnlInterview is not null)
        {
            var tag = (CmbNewStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            PnlInterview.Visibility = tag == "Interview"
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void InitInterviewForm()
    {
        DpInterviewDate.SelectedDate = DateTime.Today.AddDays(1);

        for (int h = 8; h <= 18; h++)
            CmbHour.Items.Add(new ComboBoxItem { Content = h.ToString("D2"), Tag = h });
        CmbHour.SelectedIndex = 1;

        foreach (var m in new[] { 0, 15, 30, 45 })
            CmbMinute.Items.Add(new ComboBoxItem { Content = m.ToString("D2"), Tag = m });
        CmbMinute.SelectedIndex = 0;

        InitDurationCombo();
        InitInterviewTypeCombo();
        TxtLocation.Text = Loc.T("Detail_LocationDefault");
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

        for (var i = 0; i < CmbDuration.Items.Count; i++)
        {
            if (CmbDuration.Items[i] is ComboBoxItem item &&
                (item.Tag?.ToString() ?? "") == selected)
            {
                CmbDuration.SelectedIndex = i;
                return;
            }
        }
        CmbDuration.SelectedIndex = 0;
    }

    private void InitInterviewTypeCombo()
    {
        var selected = (CmbInterviewType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Onsite";

        CmbInterviewType.Items.Clear();
        CmbInterviewType.Items.Add(new ComboBoxItem
            { Content = Loc.T("Detail_TypeOnsite"), Tag = "Onsite" });
        CmbInterviewType.Items.Add(new ComboBoxItem
            { Content = Loc.T("Detail_TypeOnline"), Tag = "Online" });

        for (var i = 0; i < CmbInterviewType.Items.Count; i++)
        {
            if (CmbInterviewType.Items[i] is ComboBoxItem item &&
                (item.Tag?.ToString() ?? "") == selected)
            {
                CmbInterviewType.SelectedIndex = i;
                return;
            }
        }
        CmbInterviewType.SelectedIndex = 0;
    }

    private void CmbNewStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_statusComboReady || PnlInterview is null) return;
        var tag = (CmbNewStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString();
        PnlInterview.Visibility = tag == "Interview"
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void CmbInterviewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateLocationForType();

    private void UpdateLocationForType()
    {
        if (LblLocation is null || TxtLocation is null) return;
        var type = (CmbInterviewType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Onsite";
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

    private async void BtnSaveStatus_Click(object sender, RoutedEventArgs e)
    {
        var status = (CmbNewStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString();
        if (string.IsNullOrEmpty(status)) return;

        if (status == "Interview")
        {
            if (DpInterviewDate.SelectedDate is null)
            {
                MessageBox.Show(Loc.T("Msg_SelectInterviewDate"));
                return;
            }

            if (_api.CurrentUserId is null)
            {
                MessageBox.Show(Loc.T("Msg_NoUser"));
                return;
            }

            var hour = int.Parse((CmbHour.SelectedItem as ComboBoxItem)!.Tag!.ToString()!);
            var minute = int.Parse((CmbMinute.SelectedItem as ComboBoxItem)!.Tag!.ToString()!);
            var dur = int.Parse((CmbDuration.SelectedItem as ComboBoxItem)!.Tag!.ToString()!);
            var type = (CmbInterviewType.SelectedItem as ComboBoxItem)!.Tag!.ToString()!;

            var scheduledAt = DpInterviewDate.SelectedDate.Value.Date
                .AddHours(hour).AddMinutes(minute);

            var req = new CreateInterviewRequest(
                _detail.Id,
                _api.CurrentUserId.Value,
                scheduledAt, dur, type,
                TxtLocation.Text.Trim());

            try
            {
                var result = await _api.CreateInterviewAsync(req);
                if (result is null)
                {
                    MessageBox.Show(Loc.T("Msg_InterviewFailed"));
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Loc.F("Msg_Error", ex.Message));
                return;
            }
        }

        var ok = await _api.UpdateApplicationStatusAsync(
            _detail.Id, status,
            string.IsNullOrWhiteSpace(TxtNote.Text)
                ? null : TxtNote.Text.Trim());

        if (ok)
        {
            var msg = status == "Interview"
                ? Loc.T("Msg_InterviewAndStatusSuccess")
                : Loc.T("Msg_SaveSuccess");
            MessageBox.Show(msg, Loc.T("Msg_SuccessTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        else
            MessageBox.Show(Loc.T("Msg_Retry"), Loc.T("Msg_Error"),
                MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private sealed record PreviewSection(string Title, List<PreviewField> Fields);
    private sealed record PreviewField(string Label, string Value);
}
