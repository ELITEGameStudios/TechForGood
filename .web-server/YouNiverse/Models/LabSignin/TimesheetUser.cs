namespace YouNiverse.Models.LabSignin;

public class TimesheetUser
{
	// Required
	public int Id { get; set; }

	public string? FirstName { get; set; }
	public string? LastName { get; set; }

	public ICollection<TimeEntry> TimeEntries { get; set; }
		= [];
}