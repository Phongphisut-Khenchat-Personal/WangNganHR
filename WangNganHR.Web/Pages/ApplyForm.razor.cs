using WangNganHR.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WangNganHR.Web.Pages;

public partial class ApplyForm : IDisposable
{
    private const int StepCount = 5;
    private const long MaxFileBytes = 5 * 1024 * 1024;

    [Parameter] public string JobId { get; set; } = "";

    [Inject] private ApiService Api { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private LocaleService Locale { get; set; } = default!;

    private JobPostingItem? _job;
    private FormModel _form = new();
    private int _step;
    private bool _loading = true;
    private bool _submitting;
    private bool _submitted;
    private string _refCode = "";
    private string _error = "";

    private IBrowserFile? _resumeFile;
    private IBrowserFile? _photoFile;
    private readonly List<IBrowserFile> _certFiles = [];

    protected override void OnInitialized() =>
        Locale.Changed += OnLocaleChanged;

    protected override async Task OnInitializedAsync()
    {
        try { _job = await Api.GetJobByIdAsync(JobId); }
        finally { _loading = false; }
    }

    private void OnLocaleChanged() => InvokeAsync(StateHasChanged);

    public void Dispose() => Locale.Changed -= OnLocaleChanged;

    private string GetStepLabel(int index) => index switch
    {
        0 => Locale.T("Apply_Section_Personal"),
        1 => Locale.T("Apply_Section_Education"),
        2 => Locale.T("Apply_Section_Work"),
        3 => Locale.T("Apply_Section_Documents"),
        _ => Locale.T("Apply_Step_Review")
    };

    private void OnSameAddressChanged(bool value)
    {
        _form.SameAsCurrentAddress = value;
        if (value) _form.RegisteredAddress = _form.Address;
    }

    private void OnFreshGraduateChanged(bool value)
    {
        _form.IsFreshGraduate = value;
        if (value) _form.WorkExperiences.Clear();
        else if (_form.WorkExperiences.Count == 0)
            _form.WorkExperiences.Add(new WorkExperienceEntry());
    }

    private static bool? ParseBool(string? v) => v switch
    {
        "true" => true,
        "false" => false,
        _ => null
    };

    private void AddWorkExperience() =>
        _form.WorkExperiences.Add(new WorkExperienceEntry());

    private void RemoveWorkExperience(int index)
    {
        if (_form.WorkExperiences.Count > 1)
            _form.WorkExperiences.RemoveAt(index);
    }

    private void NextStep()
    {
        if (!ValidateStep(_step)) return;
        _error = "";
        if (_step < StepCount - 1)
        {
            _step++;
            if (_step == StepCount - 1) BuildReviewBlocks();
        }
    }

    private void PrevStep()
    {
        _error = "";
        if (_step > 0) _step--;
    }

    private async Task HandleSubmit()
    {
        if (!ValidateAllSteps()) return;

        _submitting = true;
        _error = "";

        try
        {
            var registered = _form.SameAsCurrentAddress
                ? _form.Address?.Trim()
                : _form.RegisteredAddress?.Trim();

            var work = _form.IsFreshGraduate
                ? null
                : _form.WorkExperiences
                    .Where(w => !string.IsNullOrWhiteSpace(w.CompanyName))
                    .Select(w => new WorkExperienceForm(
                        w.CompanyName.Trim(), w.Position.Trim(),
                        w.StartDate, w.EndDate,
                        w.Responsibilities?.Trim(), w.ReasonForLeaving?.Trim(),
                        w.Salary))
                    .ToList();

            var form = new ApplicationForm(
                Guid.Parse(JobId),
                NullIfEmpty(_form.TitlePrefix),
                _form.FirstName.Trim(), _form.LastName.Trim(),
                _form.Phone.Trim(), NullIfEmpty(_form.Email),
                NullIfEmpty(_form.NationalId),
                NullIfEmpty(_form.Gender),
                NullIfEmpty(_form.NationalityType),
                NullIfEmpty(_form.LineId),
                _form.Birthdate,
                NullIfEmpty(_form.Address),
                registered,
                _form.SameAsCurrentAddress,
                NullIfEmpty(_form.Province),
                NullIfEmpty(_form.District),
                NullIfEmpty(_form.PostalCode),
                NullIfEmpty(_form.EmergencyContactName),
                NullIfEmpty(_form.EmergencyContactPhone),
                NullIfEmpty(_form.EmergencyContactRelation),
                _form.EducationLevel,
                NullIfEmpty(_form.EducationField),
                NullIfEmpty(_form.SchoolName),
                _form.GraduationYear,
                _form.Gpa,
                NullIfEmpty(_form.SkillThai),
                NullIfEmpty(_form.SkillEnglish),
                NullIfEmpty(_form.SkillJapanese),
                NullIfEmpty(_form.ComputerSkills),
                ParseBool(_form.HasDriversLicense),
                ParseBool(_form.HasOwnVehicle),
                _form.ExperienceYears,
                _form.IsFreshGraduate,
                _form.SalaryExpected,
                _form.LastSalary,
                _form.AvailableDate,
                ParseBool(_form.WillingShiftWork),
                ParseBool(_form.WillingOvertime),
                ParseBool(_form.WillingRelocate),
                "Web",
                NullIfEmpty(_form.ReferralSource),
                _form.PdpaConsented,
                work);

            var result = await Api.SubmitApplicationAsync(form);
            if (result is null)
            {
                _error = Locale.T("Apply_Error");
                return;
            }

            await UploadPendingFiles(result.Id);

            _refCode = result.ReferenceCode;
            _submitted = true;
        }
        catch { _error = Locale.T("Apply_ErrorNetwork"); }
        finally { _submitting = false; }
    }

    private async Task UploadPendingFiles(Guid applicationId)
    {
        if (_resumeFile is not null)
            await UploadOne(applicationId, "Resume", _resumeFile);
        if (_photoFile is not null)
            await UploadOne(applicationId, "Photo", _photoFile);
        foreach (var f in _certFiles)
            await UploadOne(applicationId, "Certificate", f);
    }

    private async Task UploadOne(Guid applicationId, string type, IBrowserFile file)
    {
        await using var stream = file.OpenReadStream(MaxFileBytes);
        await Api.UploadDocumentAsync(applicationId, type, stream, file.Name);
    }

    private Task OnResumeSelected(InputFileChangeEventArgs e) =>
        SetFile(e, f => _resumeFile = f);

    private Task OnPhotoSelected(InputFileChangeEventArgs e) =>
        SetFile(e, f => _photoFile = f);

    private Task OnCertSelected(InputFileChangeEventArgs e)
    {
        foreach (var f in e.GetMultipleFiles(5))
        {
            if (f.Size <= MaxFileBytes)
                _certFiles.Add(f);
        }
        return Task.CompletedTask;
    }

    private Task SetFile(InputFileChangeEventArgs e, Action<IBrowserFile?> assign)
    {
        var file = e.File;
        if (file.Size > MaxFileBytes)
        {
            _error = Locale.T("Apply_FileTooLarge");
            assign(null);
        }
        else
        {
            _error = "";
            assign(file);
        }
        return Task.CompletedTask;
    }

    private void ClearResume() => _resumeFile = null;
    private void ClearPhoto() => _photoFile = null;
    private void ClearCert(IBrowserFile f) => _certFiles.Remove(f);

    private bool ValidateAllSteps()
    {
        for (var i = 0; i < StepCount; i++)
        {
            if (!ValidateStep(i)) { _step = i; return false; }
        }
        _error = "";
        return true;
    }

    private bool ValidateStep(int step)
    {
        switch (step)
        {
            case 0:
                if (string.IsNullOrWhiteSpace(_form.FirstName)
                    || string.IsNullOrWhiteSpace(_form.LastName)
                    || string.IsNullOrWhiteSpace(_form.Phone)
                    || _form.Birthdate is null
                    || string.IsNullOrWhiteSpace(_form.NationalId)
                    || _form.NationalId.Trim().Length != 13)
                {
                    _error = Locale.T("Apply_ValidationStep1");
                    return false;
                }
                break;
            case 1:
                if (string.IsNullOrWhiteSpace(_form.EducationLevel))
                {
                    _error = Locale.T("Apply_ValidationRequired");
                    return false;
                }
                break;
            case 2:
                if (_form.AvailableDate is null
                    || ParseBool(_form.WillingShiftWork) is null
                    || ParseBool(_form.WillingOvertime) is null)
                {
                    _error = Locale.T("Apply_ValidationStep3");
                    return false;
                }
                if (!_form.IsFreshGraduate)
                {
                    var valid = _form.WorkExperiences.Any(w =>
                        !string.IsNullOrWhiteSpace(w.CompanyName)
                        && !string.IsNullOrWhiteSpace(w.Position));
                    if (!valid)
                    {
                        _error = Locale.T("Apply_ValidationWorkExp");
                        return false;
                    }
                }
                break;
            case 3:
                break;
            case 4:
                if (!_form.PdpaConsented)
                {
                    _error = Locale.T("Apply_PDPA_Required");
                    return false;
                }
                break;
        }

        _error = "";
        return true;
    }

    private static string? NullIfEmpty(string? v) =>
        string.IsNullOrWhiteSpace(v) ? null : v.Trim();

    public class FormModel
    {
        public string TitlePrefix { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string? Email { get; set; }
        public string NationalId { get; set; } = "";
        public string Gender { get; set; } = "";
        public string NationalityType { get; set; } = "Thai";
        public string? LineId { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string? Address { get; set; }
        public string? RegisteredAddress { get; set; }
        public bool SameAsCurrentAddress { get; set; } = true;
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? PostalCode { get; set; }
        public string EmergencyContactName { get; set; } = "";
        public string EmergencyContactPhone { get; set; } = "";
        public string? EmergencyContactRelation { get; set; }
        public string EducationLevel { get; set; } = "";
        public string? EducationField { get; set; }
        public string? SchoolName { get; set; }
        public int? GraduationYear { get; set; }
        public decimal? Gpa { get; set; }
        public string SkillThai { get; set; } = "Good";
        public string SkillEnglish { get; set; } = "Basic";
        public string SkillJapanese { get; set; } = "None";
        public string? ComputerSkills { get; set; }
        public string HasDriversLicense { get; set; } = "";
        public string HasOwnVehicle { get; set; } = "";
        public int ExperienceYears { get; set; }
        public bool IsFreshGraduate { get; set; }
        public decimal? SalaryExpected { get; set; }
        public decimal? LastSalary { get; set; }
        public DateOnly? AvailableDate { get; set; }
        public string WillingShiftWork { get; set; } = "";
        public string WillingOvertime { get; set; } = "";
        public string WillingRelocate { get; set; } = "";
        public string ReferralSource { get; set; } = "";
        public bool PdpaConsented { get; set; }
        public List<WorkExperienceEntry> WorkExperiences { get; set; } = [new()];
    }

    public class WorkExperienceEntry
    {
        public string CompanyName { get; set; } = "";
        public string Position { get; set; } = "";
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Responsibilities { get; set; }
        public string? ReasonForLeaving { get; set; }
        public decimal? Salary { get; set; }
    }
}
