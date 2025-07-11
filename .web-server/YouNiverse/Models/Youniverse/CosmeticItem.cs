namespace YouNiverse.Models.Youniverse;

public enum EItemSlot
{
	Hat,
	Pet,
	Shoes,
	Shirt,
	Pants,
}

public class CosmeticItem
{
	// Required
	public int Id { get; set; }

	public string? Name { get; set; }
	public EItemSlot ItemSlot { get; set; }
}