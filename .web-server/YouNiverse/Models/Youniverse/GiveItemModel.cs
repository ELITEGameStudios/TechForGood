namespace YouNiverse.Models.Youniverse;

public class GiveItemModel
{
	public int StudentId { get; set; }
	public string ItemName { get; set; } = default!;

	public string[] AllItems { get; set; } = [];
}
