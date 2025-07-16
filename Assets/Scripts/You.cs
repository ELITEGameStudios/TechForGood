using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class You : MonoBehaviour
{
	// General Information
	public string studentNumber;
	public string name;
	public string year;
	public string nameTag, bio;
	public int currentFrame;
	public GameDisplay[] contributedGames;

	[SerializeField] private SpriteRenderer[] renderers;

	// Visual Data

	public CosmeticBundleClass cosmeticBundle;
	public Sprite profileImage;

	public struct GameDisplay
	{
		public string gameName;
		public Sprite gameImage;
		public string gameDescription;
		public int gameYear;
		public bool showGameInYouniverse;
	}

	public enum ItemSlot
	{
		BASE,
		HEAD,
		FACE,
		SHIRT,
		PANTS,
		SHOES,
		PET
	}

	public enum AnimationTypeEnum
	{
		WALK_FRONT,
		WALK_SIDE,
		WALK_BACK,
		IDLE_FRONT,
		IDLE_SIDE,
		IDLE_BACK
	}

	[System.Serializable]
	public class CosmeticBundleClass
	{
		// Constants
		public const int walkFrameCount = 4;
		public const int idleFrameCount = 4;
		Dictionary<int, int> lengthLookup = new Dictionary<int, int>();

		public const int spriteWidth = 48;
		public const int spriteHeight = 48;

		public Texture spriteSheet;

		// Walk
		public Sprite[] walkFront;
		public Sprite[] walkSide;
		public Sprite[] walkBack;

		// Idle
		public Sprite[] idleFront;
		public Sprite[] idleSide;
		public Sprite[] idleBack;

		public CosmeticBundleClass(Texture source)
		{

			// Initialiing variables
			walkFront = new Sprite[walkFrameCount];
			walkSide = new Sprite[walkFrameCount];
			walkBack = new Sprite[walkFrameCount];

			idleFront = new Sprite[idleFrameCount];
			idleSide = new Sprite[idleFrameCount];
			idleBack = new Sprite[idleFrameCount];

			Sprite[][] spriteLists = new Sprite[][] { walkFront, walkSide, walkBack, idleFront, idleSide, idleBack };

			// Getting spritesheet parameters
			spriteSheet = (Texture2D)source;
			int sheetWidth = spriteSheet.width;
			int sheetHeight = spriteSheet.height;

			// AnimationTypeEnum anim = AnimationTypeEnum.WALK_FRONT;

			// Maps each sprite to its assigned x,y coordinates given based on which list and frame it is
			for (int y = 0; y < spriteLists.Length; y++)
			{
				for (int x = 0; x < spriteLists[y].Length; x++)
				{
					spriteLists[y][x] = (
						Sprite.Create(
							(Texture2D)spriteSheet,
							new Rect(x * sheetWidth, sheetHeight - y * sheetHeight, spriteWidth, spriteHeight),
							Vector2.zero
						)
					);
				}
			}
		}
	}

	// Time stats
	public float secondsPlayed;
	public float minutesPlayed { get { return secondsPlayed / 60; } }
	public float hoursPlayed { get { return minutesPlayed / 60; } }

	public void SetData(YouWebClass youWebClass, YouCosmeticData cosmeticData, string studentNumber)
	{ // , CosmeticBundleStruct cosmetics){
		Debug.Log(youWebClass.FirstName + " " + youWebClass.LastName + " " + studentNumber);

		this.studentNumber = studentNumber;
		name = youWebClass.FirstName + " " + youWebClass.LastName;
		nameTag = youWebClass.Catchphrase;
		bio = youWebClass.Catchphrase;
		year = youWebClass.Year;

		for (int i = 0; i < renderers.Length; i++)
		{
			// renderers[i].sprite.texture = cosmeticData.bundleStruct[i].front;
		}


		// cosmeticBundle = cosmetics;
		// this.profileImage = profileImage;
	}

	public void Retire()
	{
		Destroy(gameObject);
	}



}
