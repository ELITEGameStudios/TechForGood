namespace YouNiverse.Models.Youniverse;

public class UserItem
{
	// Required
	public int Id { get; set; }

	public string? FirstName { get; set; }
	public string? LastName { get; set; }

	public bool HasYou { get; set; }
}