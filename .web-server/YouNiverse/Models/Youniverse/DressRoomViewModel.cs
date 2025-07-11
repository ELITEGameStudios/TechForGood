namespace YouNiverse.Models.Youniverse;

public class DressRoomViewModel
{
	public CategoryData[] Categories { get; set; } = [];

	public struct CategoryData
	{
		public CosmeticItem[] UnlockedItems;
	}
}