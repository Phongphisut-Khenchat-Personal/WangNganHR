namespace WangNganHR.Web.Pages;

public partial class ApplyForm
{
    private sealed record ReviewBlock(string Title, List<ReviewField> Fields);
    private sealed record ReviewField(string Label, string Value);

    private List<ReviewBlock> _reviewBlocks = [];

    private void BuildReviewBlocks()
    {
        var blocks = new List<ReviewBlock>();

        blocks.Add(new ReviewBlock(Locale.T("Apply_Section_Personal"), [
            Field(Locale.T("Apply_TitlePrefix"), LabelPrefix(_form.TitlePrefix)),
            Field(Locale.T("Apply_FirstName"), _form.FirstName),
            Field(Locale.T("Apply_LastName"), _form.LastName),
            Field(Locale.T("Apply_Gender"), LabelGender(_form.Gender)),
            Field(Locale.T("Apply_NationalId"), _form.NationalId),
            Field(Locale.T("Apply_Birthdate"), FmtDate(_form.Birthdate)),
            Field(Locale.T("Apply_Phone"), _form.Phone),
            Field(Locale.T("Apply_Email"), Fmt(_form.Email)),
            Field(Locale.T("Apply_LineId"), Fmt(_form.LineId)),
            Field(Locale.T("Apply_Nationality"), LabelNationality(_form.NationalityType)),
            Field(Locale.T("Apply_Address"), Fmt(_form.Address)),
            Field(Locale.T("Apply_Province"), FmtJoin(_form.District, _form.Province, _form.PostalCode)),
            Field(Locale.T("Apply_RegisteredAddress"),
                _form.SameAsCurrentAddress ? Locale.T("Apply_SameAddress") : Fmt(_form.RegisteredAddress)),
            Field(Locale.T("Apply_EmergencyName"), Fmt(_form.EmergencyContactName)),
            Field(Locale.T("Apply_EmergencyPhone"), Fmt(_form.EmergencyContactPhone)),
            Field(Locale.T("Apply_EmergencyRelation"), Fmt(_form.EmergencyContactRelation)),
        ]));

        blocks.Add(new ReviewBlock(Locale.T("Apply_Section_Education"), [
            Field(Locale.T("Apply_Education"), LabelEducation(_form.EducationLevel)),
            Field(Locale.T("Apply_Field"), Fmt(_form.EducationField)),
            Field(Locale.T("Apply_SchoolName"), Fmt(_form.SchoolName)),
            Field(Locale.T("Apply_GraduationYear"), _form.GraduationYear?.ToString() ?? "–"),
            Field(Locale.T("Apply_Gpa"), _form.Gpa?.ToString("0.00") ?? "–"),
            Field(Locale.T("Apply_SkillThai"), LabelSkill(_form.SkillThai)),
            Field(Locale.T("Apply_SkillEnglish"), LabelSkill(_form.SkillEnglish)),
            Field(Locale.T("Apply_SkillJapanese"), LabelSkill(_form.SkillJapanese)),
            Field(Locale.T("Apply_ComputerSkills"), Fmt(_form.ComputerSkills)),
            Field(Locale.T("Apply_DriversLicense"), FmtBoolStr(_form.HasDriversLicense)),
            Field(Locale.T("Apply_OwnVehicle"), FmtBoolStr(_form.HasOwnVehicle)),
        ]));

        var workFields = new List<ReviewField>
        {
            Field(Locale.T("Apply_AvailableDate"), FmtDate(_form.AvailableDate)),
            Field(Locale.T("Apply_Experience"), _form.ExperienceYears.ToString()),
            Field(Locale.T("Apply_FreshGraduate"), _form.IsFreshGraduate ? Locale.T("Yes") : Locale.T("No")),
            Field(Locale.T("Apply_Salary"), FmtMoney(_form.SalaryExpected)),
            Field(Locale.T("Apply_LastSalary"), FmtMoney(_form.LastSalary)),
            Field(Locale.T("Apply_ShiftWork"), FmtBoolStr(_form.WillingShiftWork)),
            Field(Locale.T("Apply_Overtime"), FmtBoolStr(_form.WillingOvertime)),
            Field(Locale.T("Apply_Relocate"), FmtBoolStr(_form.WillingRelocate)),
        };
        blocks.Add(new ReviewBlock(Locale.T("Apply_Section_Work"), workFields));

        if (!_form.IsFreshGraduate)
        {
            for (var i = 0; i < _form.WorkExperiences.Count; i++)
            {
                var w = _form.WorkExperiences[i];
                if (string.IsNullOrWhiteSpace(w.CompanyName)) continue;
                blocks.Add(new ReviewBlock(Locale.F("Apply_WorkItem", i + 1), [
                    Field(Locale.T("Apply_Company"), w.CompanyName),
                    Field(Locale.T("Apply_Position"), w.Position),
                    Field(Locale.T("Apply_StartDate"), FmtDate(w.StartDate)),
                    Field(Locale.T("Apply_EndDate"), FmtDate(w.EndDate)),
                    Field(Locale.T("Apply_Responsibilities"), Fmt(w.Responsibilities)),
                    Field(Locale.T("Apply_LeaveReason"), Fmt(w.ReasonForLeaving)),
                    Field(Locale.T("Apply_JobSalary"), FmtMoney(w.Salary)),
                ]));
            }
        }

        var docFields = new List<ReviewField>
        {
            Field(Locale.T("Apply_Doc_Resume"), _resumeFile?.Name ?? Locale.T("Apply_Doc_None")),
            Field(Locale.T("Apply_Doc_Photo"), _photoFile?.Name ?? Locale.T("Apply_Doc_None")),
            Field(Locale.T("Apply_Doc_Certificate"),
                _certFiles.Count > 0
                    ? string.Join(", ", _certFiles.Select(f => f.Name))
                    : Locale.T("Apply_Doc_None")),
        };
        blocks.Add(new ReviewBlock(Locale.T("Apply_Section_Documents"), docFields));

        if (!string.IsNullOrWhiteSpace(_form.ReferralSource))
            blocks.Add(new ReviewBlock(Locale.T("Apply_ReferralSource"), [
                Field(Locale.T("Apply_ReferralSource"), LabelReferral(_form.ReferralSource)),
            ]));

        _reviewBlocks = blocks;
    }

    private static ReviewField Field(string label, string value) => new(label, value);

    private static string Fmt(string? v) =>
        string.IsNullOrWhiteSpace(v) ? "–" : v.Trim();

    private static string FmtDate(DateOnly? d) => d?.ToString("dd/MM/yyyy") ?? "–";

    private string FmtMoney(decimal? v) =>
        v.HasValue ? Locale.F("Apply_MoneyFormat", v.Value) : "–";

    private string FmtBoolStr(string? v) => ParseBool(v) switch
    {
        true => Locale.T("Yes"),
        false => Locale.T("No"),
        _ => "–"
    };

    private static string FmtJoin(params string?[] parts)
    {
        var joined = string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p!.Trim()));
        return string.IsNullOrWhiteSpace(joined) ? "–" : joined;
    }

    private string LabelPrefix(string? v) => v switch
    {
        "Mr" => Locale.T("Prefix_Mr"),
        "Mrs" => Locale.T("Prefix_Mrs"),
        "Miss" => Locale.T("Prefix_Miss"),
        _ => "–"
    };

    private string LabelGender(string? v) => v switch
    {
        "Male" => Locale.T("Gender_Male"),
        "Female" => Locale.T("Gender_Female"),
        "Other" => Locale.T("Gender_Other"),
        _ => "–"
    };

    private string LabelNationality(string? v) => v switch
    {
        "Thai" => Locale.T("Nationality_Thai"),
        "Foreign" => Locale.T("Nationality_Foreign"),
        "MigrantWorker" => Locale.T("Nationality_Migrant"),
        _ => "–"
    };

    private string LabelEducation(string? v) => v switch
    {
        "BelowMiddleSchool" => Locale.T("Edu_BelowMiddle"),
        "MiddleSchool" => Locale.T("Edu_Middle"),
        "HighSchool" => Locale.T("Edu_HighSchool"),
        "Vocational" => Locale.T("Edu_Vocational"),
        "HighVocational" => Locale.T("Edu_HighVocational"),
        "Bachelor" => Locale.T("Edu_Bachelor"),
        "Master" => Locale.T("Edu_Master"),
        "Doctor" => Locale.T("Edu_Doctor"),
        null or "" => "–",
        _ => v
    };

    private string LabelSkill(string? v) => string.IsNullOrWhiteSpace(v)
        ? "–"
        : Locale.T($"Skill_{v}");

    private string LabelReferral(string? v) => v switch
    {
        "JobBoard" => Locale.T("Referral_JobBoard"),
        "Social" => Locale.T("Referral_Social"),
        "Friend" => Locale.T("Referral_Friend"),
        "Company" => Locale.T("Referral_Company"),
        "QR" => Locale.T("Referral_QR"),
        "WalkIn" => Locale.T("Referral_WalkIn"),
        "Other" => Locale.T("Referral_Other"),
        _ => "–"
    };
}
