namespace YouNiverse.Models.Youniverse;

public class DressRoomViewModel
{
	public CategoryData[] UnlockedCategories { get; set; } = [];
	public int[] SelectedItems { get; set; } = [];
	public string SkinColor { get; set; } = "#00FF00";

	public struct CategoryData
	{
		public CosmeticItem[] UnlockedItems;
	}
}