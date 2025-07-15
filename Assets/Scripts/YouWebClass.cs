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
    // public int BaseItemId = -1;
    public int HeadItemId = -1;
    public int FaceItemId = -1;
    public int ShirtItemId = -1;
    public int PantsItemId = -1;
    public int ShoesItemId= -1;
    public int PetItemId = -1;
    public int[] IdList { get { return new int[6] { HeadItemId, FaceItemId, ShirtItemId, PantsItemId, ShoesItemId, PetItemId }; } }

    public CosmeticBundleClass[] bundleStruct;
}
