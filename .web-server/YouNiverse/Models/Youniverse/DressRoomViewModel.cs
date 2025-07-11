namespace YouNiverse.Models.Youniverse;

public class DressRoomViewModel
{
	CategoryData[] Categories { get; set; } = [
		new CategoryData(){
			CategoryName = "Hats",
		},
		new CategoryData(){
			CategoryName = "Shoes",
		},
		new CategoryData(){
			CategoryName = "Pants",
		},
	];

	public struct CategoryData
	{
		public string CategoryName { get; set; }
		public EItemSlot ItemSlot { get; set; }
	}
}