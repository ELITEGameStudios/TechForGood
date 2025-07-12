namespace YouNiverse.Models.Youniverse;

public class DressRoomViewModel
{
	public CategoryData[] UnlockedCategories { get; set; } = [];
	public int[] SelectedItems { get; set; } = [];

	public struct CategoryData
	{
		public CosmeticItem[] UnlockedItems;
	}
}