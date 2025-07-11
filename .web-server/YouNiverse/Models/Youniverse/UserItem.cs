namespace YouNiverse.Models.Youniverse;

public class UserItem
{
	// Required
	public int Id { get; set; }

	public string FirstName { get; set; } = default!;
	public string LastName { get; set; } = default!;

	public ICollection<UnlockEntry> Unlocks { get; set; } = [];

	public ItemLoadout Loadout { get; set; } = new ItemLoadout();
}

public class ItemLoadout
{
	public int HeadItemId { get; set; } = -1;
	public int FaceItemId { get; set; } = -1;
	public int ShirtItemId { get; set; } = -1;
	public int PantsItemId { get; set; } = -1;
	public int ShoesItemId { get; set; } = -1;
	public int PetItemId { get; set; } = -1;
}