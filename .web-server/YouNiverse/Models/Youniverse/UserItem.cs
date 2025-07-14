using System.ComponentModel.DataAnnotations;

namespace YouNiverse.Models.Youniverse;

public class UserItem
{
	public const string k_DefaultPrimaryColor = "#79bac9";
	public const string k_DefaultSecondaryColor = "#eaaf5a";

	// Required
	public int Id { get; set; }

	public string FirstName { get; set; } = default!;
	public string LastName { get; set; } = default!;

	public ICollection<UnlockEntry> Unlocks { get; set; } = [];

	public ItemLoadout Loadout { get; set; } = new ItemLoadout();

	public string? Catchphrase { get; set; }
	public EGameRole Role { get; set; } = EGameRole.Other;

	public string PrimaryColor { get; set; } = k_DefaultPrimaryColor;
	public string SecondaryColor { get; set; } = k_DefaultSecondaryColor;

	public float Hours { get; set; }
}

public enum EGameRole
{
	[Display(Name = "Programmer")]
	Programmer,
	[Display(Name = "2D Artist")]
	Artist2D,
	[Display(Name = "3D Artist")]
	Artist3D,
	[Display(Name = "Designer")]
	Designer,
	[Display(Name = "Sound Designer")]
	SoundDesigner,
	[Display(Name = "Other")]
	Other,
}

public class ItemLoadout
{
	public int HeadItemId { get; set; } = -1;
	public int FaceItemId { get; set; } = -1;
	public int ShirtItemId { get; set; } = -1;
	public int PantsItemId { get; set; } = -1;
	public int ShoesItemId { get; set; } = -1;
	public int PetItemId { get; set; } = -1;

	public int FromSlotIndex(int i)
	{
		EItemSlot slot = (EItemSlot)i;
		return slot switch
		{
			EItemSlot.Head => HeadItemId,
			EItemSlot.Face => FaceItemId,
			EItemSlot.Shirt => ShirtItemId,
			EItemSlot.Pants => PantsItemId,
			EItemSlot.Shoes => ShoesItemId,
			EItemSlot.Pet => PetItemId,
			_ => -1,
		};
	}

	public void SetWithSlotIndex(int item, EItemSlot slot)
	{
		switch (slot)
		{
			case EItemSlot.Head:
				HeadItemId = item;
				break;
			case EItemSlot.Face:
				FaceItemId = item;
				break;
			case EItemSlot.Shirt:
				ShirtItemId = item;
				break;
			case EItemSlot.Pants:
				PantsItemId = item;
				break;
			case EItemSlot.Shoes:
				ShoesItemId = item;
				break;
			case EItemSlot.Pet:
				PetItemId = item;
				break;
			default:
				break;
		}
	}
}