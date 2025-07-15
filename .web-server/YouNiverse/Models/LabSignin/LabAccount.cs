namespace YouNiverse.Models.LabSignin;

public enum EAccountType
{
	None,
	LabGuest,
	LabAndYouniverse
}

public class LabAccount
{
	// Required
	public int Id { get; set; }

	public EAccountType AccountType { get; set; }

	public int? StudentId { get; set; }

	public string FirstName { get; set; } = default!;
	public string LastName { get; set; } = default!;

	public float Hours { get; set; }

	public ICollection<TimeEntry> TimeEntries { get; set; } = [];
}