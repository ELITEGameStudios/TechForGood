using UnityEngine;
using static You;

public class ProfileData
{

	// General Information
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Catchphrase { get; set; }
	public string Role { get; set; }
	public string PrimaryColor { get; set; }
	public string SecondaryColor { get; set; }
	public float Hours { get; set; }
	public string Bio { get; set; }
	public int Year { get; set; }
	public string Team { get; set; }

	// public string nameTag;
	// public float secondsPlayed;
	// Cosmetic Information
	// public int Cosmetic;
	// public int hatCosmetic;
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

public class AvatarData
{
	public Loadout Loadout { get; set; } = default;
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
