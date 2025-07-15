namespace YouNiverse.Models.Youniverse;

public class UnlockEntry
{
	// Required
	public int Id { get; set; }

	public int UserId { get; set; }
	public YouAccount? User { get; set; }

	public int ItemId { get; set; }
	public CosmeticItem Item { get; set; } = default!;

	public DateTime UnlockDate { get; set; }
}