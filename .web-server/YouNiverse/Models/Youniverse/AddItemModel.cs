namespace YouNiverse.Models.Youniverse;

public class AddItemModel
{
	public string? ItemName { get; set; }
	public EItemSlot ItemSlot { get; set; }

	public IFormFile? FrontImage { get; set; }
	public IFormFile? SideImage { get; set; }
	public IFormFile? BackImage { get; set; }
}
