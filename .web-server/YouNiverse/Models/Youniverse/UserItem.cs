namespace YouNiverse.Models.Youniverse;

public class UserItem
{
	// Required
	public int Id { get; set; }

	public string FirstName { get; set; } = default!;
	public string LastName { get; set; } = default!;

	public ICollection<UnlockEntry> Unlocks { get; set; } = [];

	public string EquippedItems { get; set; } = "";
}