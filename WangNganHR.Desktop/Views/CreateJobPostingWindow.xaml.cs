using WangNganHR.Desktop.Localization;
using WangNganHR.Desktop.Models;
using WangNganHR.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace WangNganHR.Desktop.Views;

public partial class CreateJobPostingWindow : Window
{
    private readonly ApiService _api;
    private readonly Guid? _editId;
    private int _step;

    public CreateJobPostingWindow(ApiService api, Guid? editId = null)
    {
        InitializeComponent();
        _api = api;
        _editId = editId;
        Title = editId is null ? Loc.T("CreateJob_Title") : Loc.T("EditJob_Title");
        InitDeptCombo();
        InitBulletLists();
        if (editId is not null)
        {
            ChkPublishNow.Visibility = Visibility.Collapsed;
            _ = LoadForEditAsync(editId.Value);
        }
        SetStep(0);
        LocalizationService.Instance.LanguageChanged += OnLanguageChanged;
        Closed += (_, _) => LocalizationService.Instance.LanguageChanged -= OnLanguageChanged;
    }

    private async Task LoadForEditAsync(Guid id)
    {
        try
        {
            var job = await _api.GetJobPostingAsync(id);
            if (job is null) { ShowError(Loc.T("Msg_Retry")); return; }

            for (var i = 0; i < CmbDept.Items.Count; i++)
            {
                if (CmbDept.Items[i] is ComboBoxItem item &&
                    item.Tag?.ToString() == job.DepartmentId.ToString())
                {
                    CmbDept.SelectedIndex = i;
                    break;
                }
            }

            TxtTitle.Text = job.Title;
            TxtSalMin.Text = job.SalaryMin?.ToString("0") ?? "";
            TxtSalMax.Text = job.SalaryMax?.ToString("0") ?? "";
            TxtCount.Text = job.PositionsCount.ToString();
            TxtWorkHours.Text = job.WorkHours ?? "";
            TxtWorkLocation.Text = job.WorkLocation ?? "";
            ListResponsibilities.SetItems(job.Responsibilities);
            ListQualifications.SetItems(job.Qualifications);
            ListBenefits.SetItems(job.Benefits);
        }
        catch (Exception ex)
        {
            ShowError(Loc.F("Msg_Error", ex.Message));
        }
    }

    private void OnLanguageChanged() => Dispatcher.Invoke(RefreshLocalizedUi);

    private void RefreshLocalizedUi()
    {
        InitDeptCombo();
        InitBulletLists();
        if (_step == 1) BuildPreview();
        SetStep(_step);
    }

    private void InitBulletLists()
    {
        var resp = ListResponsibilities.GetItems();
        var qual = ListQualifications.GetItems();
        var bene = ListBenefits.GetItems();

        ListResponsibilities.SetCaption(Loc.T("CreateJob_Responsibilities"));
        ListResponsibilities.SetAddLabel(Loc.T("CreateJob_AddItem"));
        ListResponsibilities.SetPlaceholder(Loc.T("Placeholder_JobBullet"));
        ListResponsibilities.SetItems(resp);

        ListQualifications.SetCaption(Loc.T("CreateJob_Qualifications"));
        ListQualifications.SetAddLabel(Loc.T("CreateJob_AddItem"));
        ListQualifications.SetPlaceholder(Loc.T("Placeholder_JobBullet"));
        ListQualifications.SetItems(qual);

        ListBenefits.SetCaption(Loc.T("CreateJob_Benefits"));
        ListBenefits.SetAddLabel(Loc.T("CreateJob_AddItem"));
        ListBenefits.SetPlaceholder(Loc.T("Placeholder_JobBullet"));
        ListBenefits.SetItems(bene);
    }

    private void InitDeptCombo()
    {
        var selected = (CmbDept.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "1";

        CmbDept.Items.Clear();
        foreach (var (key, tag) in new[]
        {
            ("Dept_Production", "1"), ("Dept_QA", "2"), ("Dept_Logistics", "3"),
            ("Dept_Maintenance", "4"), ("Dept_HR", "5"), ("Dept_Admin", "6")
        })
            CmbDept.Items.Add(new ComboBoxItem { Content = Loc.T(key), Tag = tag });

        for (var i = 0; i < CmbDept.Items.Count; i++)
        {
            if (CmbDept.Items[i] is ComboBoxItem item &&
                (item.Tag?.ToString() ?? "") == selected)
            {
                CmbDept.SelectedIndex = i;
                return;
            }
        }
        CmbDept.SelectedIndex = 0;
    }

    private void SetStep(int step)
    {
        _step = step;
        var isInput = step == 0;

        PnlInput.Visibility = isInput ? Visibility.Visible : Visibility.Collapsed;
        PnlPreview.Visibility = isInput ? Visibility.Collapsed : Visibility.Visible;

        BtnStepInput.Style = (Style)FindResource(isInput ? "CreateStepBtnActive" : "CreateStepBtn");
        BtnStepPreview.Style = (Style)FindResource(isInput ? "CreateStepBtn" : "CreateStepBtnActive");

        BtnNext.Visibility = isInput ? Visibility.Visible : Visibility.Collapsed;
        BtnBack.Visibility = isInput ? Visibility.Collapsed : Visibility.Visible;
        BtnSave.Visibility = isInput ? Visibility.Collapsed : Visibility.Visible;
    }

    private void BtnStepInput_Click(object sender, RoutedEventArgs e) => SetStep(0);

    private void BtnStepPreview_Click(object sender, RoutedEventArgs e) => GoToPreview();

    private void BtnNext_Click(object sender, RoutedEventArgs e) => GoToPreview();

    private void GoToPreview()
    {
        if (!ValidateInput()) return;
        try
        {
            BuildPreview();
            SetStep(1);
        }
        catch (Exception ex)
        {
            ShowError(Loc.F("Msg_Error", ex.Message));
        }
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e) => SetStep(0);

    private bool ValidateInput()
    {
        TxtError.Visibility = Visibility.Collapsed;

        if (string.IsNullOrWhiteSpace(TxtTitle.Text))
        {
            ShowError(Loc.T("Msg_RequiredFields"));
            return false;
        }

        if (ListResponsibilities.GetItems().Count == 0)
        {
            ShowError(Loc.T("Msg_JobResponsibilitiesRequired"));
            return false;
        }

        if (ListQualifications.GetItems().Count == 0)
        {
            ShowError(Loc.T("Msg_JobQualificationsRequired"));
            return false;
        }

        return true;
    }

    private void BuildPreview()
    {
        PreviewDept.Text = (CmbDept.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
        PreviewTitle.Text = TxtTitle.Text.Trim();

        decimal.TryParse(TxtSalMin.Text, out var salMin);
        decimal.TryParse(TxtSalMax.Text, out var salMax);
        int.TryParse(TxtCount.Text, out var count);
        if (count <= 0) count = 1;

        PreviewSalary.Text = salMin > 0
            ? Loc.F("Salary_Range", salMin, salMax > 0 ? salMax : salMin)
            : Loc.T("JobSalary_NotSpecified");
        PreviewPositions.Text = Loc.F("Job_Positions", count);

        if (!string.IsNullOrWhiteSpace(TxtWorkHours.Text))
        {
            PreviewWorkHours.Text = Loc.T("CreateJob_WorkHours") + ": " + TxtWorkHours.Text.Trim();
            PreviewWorkHours.Visibility = Visibility.Visible;
        }
        else PreviewWorkHours.Visibility = Visibility.Collapsed;

        if (!string.IsNullOrWhiteSpace(TxtWorkLocation.Text))
        {
            PreviewWorkLocation.Text = Loc.T("CreateJob_WorkLocation") + ": " + TxtWorkLocation.Text.Trim();
            PreviewWorkLocation.Visibility = Visibility.Visible;
        }
        else PreviewWorkLocation.Visibility = Visibility.Collapsed;

        var sections = new List<PreviewSection>
        {
            new(Loc.T("CreateJob_Responsibilities"), ListResponsibilities.GetItems()),
            new(Loc.T("CreateJob_Qualifications"), ListQualifications.GetItems())
        };

        var benefits = ListBenefits.GetItems();
        if (benefits.Count > 0)
            sections.Add(new(Loc.T("CreateJob_Benefits"), benefits));

        PreviewSections.ItemsSource = sections;
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput()) { SetStep(0); return; }

        TxtError.Visibility = Visibility.Collapsed;

        var deptId = int.Parse((CmbDept.SelectedItem as ComboBoxItem)!.Tag!.ToString()!);
        decimal.TryParse(TxtSalMin.Text, out var salMin);
        decimal.TryParse(TxtSalMax.Text, out var salMax);
        int.TryParse(TxtCount.Text, out var count);
        if (count <= 0) count = 1;

        var benefits = ListBenefits.GetItems();
        var req = new CreateJobPostingRequest(
            deptId,
            TxtTitle.Text.Trim(),
            ListResponsibilities.GetItems(),
            ListQualifications.GetItems(),
            benefits.Count > 0 ? benefits : null,
            string.IsNullOrWhiteSpace(TxtWorkHours.Text) ? null : TxtWorkHours.Text.Trim(),
            string.IsNullOrWhiteSpace(TxtWorkLocation.Text) ? null : TxtWorkLocation.Text.Trim(),
            salMin > 0 ? salMin : null,
            salMax > 0 ? salMax : null,
            count,
            ChkPublishNow.IsChecked == true ? DateTime.UtcNow : null,
            null);

        try
        {
            if (_editId is not null)
            {
                var update = new UpdateJobPostingRequest(
                    deptId, TxtTitle.Text.Trim(),
                    ListResponsibilities.GetItems(),
                    ListQualifications.GetItems(),
                    benefits.Count > 0 ? benefits : null,
                    string.IsNullOrWhiteSpace(TxtWorkHours.Text) ? null : TxtWorkHours.Text.Trim(),
                    string.IsNullOrWhiteSpace(TxtWorkLocation.Text) ? null : TxtWorkLocation.Text.Trim(),
                    salMin > 0 ? salMin : null,
                    salMax > 0 ? salMax : null,
                    count);

                var ok = await _api.UpdateJobPostingAsync(_editId.Value, update);
                if (ok)
                {
                    MessageBox.Show(Loc.T("Msg_UpdateJobSuccess"), Loc.T("Msg_SuccessTitle"),
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else ShowError(Loc.T("Msg_Retry"));
                return;
            }

            var result = await _api.CreateJobPostingAsync(req);
            if (result is not null)
            {
                if (ChkPublishNow.IsChecked == true)
                    await _api.PublishJobPostingAsync(result.Id);

                MessageBox.Show(Loc.T("Msg_CreateJobSuccess"), Loc.T("Msg_SuccessTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
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
        TxtError.Text = msg;
        TxtError.Visibility = Visibility.Visible;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

    private sealed record PreviewSection(string Title, List<string> Items);
}
