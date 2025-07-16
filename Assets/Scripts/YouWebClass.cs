using UnityEngine;
using static You;

public class ProfileData
{

	// General Information
	public string FirstName;
	public string LastName;
	public string Catchphrase;
	public string Role;
	public string PrimaryColor;
	public string SecondaryColor;
	public float Hours;
	public string Bio;
	public string Year;
	public string Team;

	// public string nameTag;
	// public float secondsPlayed;
	// Cosmetic Information
	// public int Cosmetic;
	// public int hatCosmetic;
}

public class AvatarData
{
	public Loadout Loadout { get; set; } = default;

	public CosmeticBundleClass[] cosmeticBundles;
}

public enum CosmeticSlot
{
	BASE,
	HEAD,
	FACE,
	SHIRT,
	PANTS,
	SHOES,
	PET
}

public class Loadout
{
	public int BaseItemId { get; set; } = -1;
	public int HeadItemId { get; set; } = -1;
	public int FaceItemId { get; set; } = -1;
	public int ShirtItemId { get; set; } = -1;
	public int PantsItemId { get; set; } = -1;
	public int ShoesItemId { get; set; } = -1;
	public int PetItemId { get; set; } = -1;

	public int GetIdForSlot(CosmeticSlot slot)
	{
		return slot switch
		{
			CosmeticSlot.BASE => BaseItemId,
			CosmeticSlot.HEAD => HeadItemId,
			CosmeticSlot.FACE => FaceItemId,
			CosmeticSlot.SHIRT => ShirtItemId,
			CosmeticSlot.PANTS => PantsItemId,
			CosmeticSlot.SHOES => ShoesItemId,
			_ => -1,
		};
	}
}
