namespace JanomeHR.Shared.Entities;

public class WorkExperience
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Responsibilities { get; set; }
    public string? ReasonForLeaving { get; set; }
    public decimal? Salary { get; set; }

    public Application Application { get; set; } = null!;
}
