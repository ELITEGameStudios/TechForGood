using UnityEngine;
using static You;

public class YouWebClass
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

public class YouCosmeticData
{
	public CosmeticLoadout Loadout { get; set; } = default;

	public CosmeticBundleClass baseData;
	public CosmeticBundleClass shirtData;
}

public class CosmeticLoadout
{
	public int BaseItemId { get; set; } = -1;
	public int HeadItemId { get; set; } = -1;
	public int FaceItemId { get; set; } = -1;
	public int ShirtItemId { get; set; } = -1;
	public int PantsItemId { get; set; } = -1;
	public int ShoesItemId { get; set; } = -1;
	public int PetItemId { get; set; } = -1;
}
