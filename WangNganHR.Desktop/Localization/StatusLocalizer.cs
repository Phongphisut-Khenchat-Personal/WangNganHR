namespace WangNganHR.Desktop.Localization;

public static class StatusLocalizer
{
    public static string ApplicationStatus(string status) =>
        status switch
        {
            "New" => Loc.T("Status_New"),
            "Review" => Loc.T("Status_Review"),
            "Interview" => Loc.T("Status_Interview"),
            "Pass" => Loc.T("Status_Pass"),
            "Fail" => Loc.T("Status_Fail"),
            _ => status
        };

    public static string ApplicationSource(string source) =>
        source switch
        {
            "QRCode" => Loc.T("Source_QRCode"),
            "WalkIn" => Loc.T("Source_WalkIn"),
            "Web" => Loc.T("Source_Web"),
            _ => source
        };

    public static string ReferralSource(string? source) => source switch
    {
        "JobBoard" => Loc.T("Referral_JobBoard"),
        "Social" => Loc.T("Referral_Social"),
        "Friend" => Loc.T("Referral_Friend"),
        "Company" => Loc.T("Referral_Company"),
        "QR" => Loc.T("Referral_QR"),
        "WalkIn" => Loc.T("Referral_WalkIn"),
        "Other" => Loc.T("Referral_Other"),
        null or "" => "–",
        _ => source
    };

    public static string InterviewType(string type) =>
        type switch
        {
            "Online" => Loc.T("Type_Online"),
            "Onsite" => Loc.T("Type_Onsite"),
            _ => type
        };

    public static string InterviewStatus(string status) =>
        status switch
        {
            "Scheduled" => Loc.T("InterviewStatus_Scheduled"),
            "Done" or "Completed" => Loc.T("InterviewStatus_Completed"),
            "Cancelled" => Loc.T("InterviewStatus_Cancelled"),
            _ => status
        };

    public static string JobPostingStatus(string status) =>
        status switch
        {
            "Active" => Loc.T("JobStatus_Active"),
            "Draft" => Loc.T("JobStatus_Draft"),
            "Closed" => Loc.T("JobStatus_Closed"),
            _ => status
        };

    public static string EducationLevel(string? level) => level switch
    {
        "BelowMiddleSchool" => Loc.T("Edu_BelowMiddle"),
        "MiddleSchool" => Loc.T("Edu_Middle"),
        "HighSchool" => Loc.T("Edu_HighSchool"),
        "Vocational" => Loc.T("Edu_Vocational"),
        "HighVocational" => Loc.T("Edu_HighVocational"),
        "Bachelor" => Loc.T("Edu_Bachelor"),
        "Master" => Loc.T("Edu_Master"),
        "Doctor" => Loc.T("Edu_Doctor"),
        null or "" => "–",
        _ => level
    };

    public static string Gender(string? value) => value switch
    {
        "Male" => Loc.T("Gender_Male"),
        "Female" => Loc.T("Gender_Female"),
        "Other" => Loc.T("Gender_Other"),
        _ => value ?? "–"
    };

    public static string Nationality(string? value) => value switch
    {
        "Thai" => Loc.T("Nationality_Thai"),
        "Foreign" => Loc.T("Nationality_Foreign"),
        "MigrantWorker" => Loc.T("Nationality_Migrant"),
        _ => value ?? "–"
    };

    public static string SkillLevel(string? value) => value switch
    {
        "None" => Loc.T("Skill_None"),
        "Basic" => Loc.T("Skill_Basic"),
        "Good" => Loc.T("Skill_Good"),
        "Fluent" => Loc.T("Skill_Fluent"),
        _ => value ?? "–"
    };

    public static string TitlePrefix(string? value) => value switch
    {
        "Mr" => Loc.T("Prefix_Mr"),
        "Mrs" => Loc.T("Prefix_Mrs"),
        "Miss" => Loc.T("Prefix_Miss"),
        _ => value ?? ""
    };

    public static string DocumentType(string type) => type switch
    {
        "Resume" => Loc.T("Doc_Resume"),
        "Photo" => Loc.T("Doc_Photo"),
        "Certificate" => Loc.T("Doc_Certificate"),
        "Other" => Loc.T("Doc_Other"),
        _ => type
    };
}
