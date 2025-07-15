namespace YouNiverse.Models.Youniverse;

public class AddItemModel
{
	public string? ItemName { get; set; }
	public EItemSlot ItemSlot { get; set; }

	public IFormFile? SpriteSheet { get; set; }

	public bool IsDefault { get; set; }
}
