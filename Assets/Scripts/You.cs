using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class You : MonoBehaviour
{
	const int spriteWidth = 48;
	const int spriteHeight = 48;
	const float middleRoomX = 6;

	[SerializeField] float profileRefreshPeriod = 5;
	[SerializeField] float lookDirChangeThreshold = 0.5f;

	[SerializeField] float minCatchphraseTime, maxCatchphraseTime;

	// Stores which slot is in which index, plus renderer info
	[SerializeField] SlotData[] cosmeticSlots;
	[SerializeField] SpriteRenderer eyesRenderer;

	readonly CancellationTokenSource tokenSrc = new();
	AvatarAI ai;
	Animator animator;
	AnimationEnum currentAnimation;
	bool isInitialized = false;

	public int currentFrame; // set by animations

	public FacingDir FacingDirection { get; set; }

	public int UserId { get; private set; }
	public ProfileData ProfileData { get; private set; }
	public AvatarData AvatarData { get; private set; }

	public Transform catchphraseCanvas;
	public Animator speechBubbleAnimator;
	[SerializeField] private Text catchphraseText;

	public SlotData[] Slots => cosmeticSlots;

	void Awake()
	{
		ai = GetComponent<AvatarAI>();
		animator = GetComponent<Animator>();
		TriggerCatchphrase();
	}

	void OnDestroy()
	{
		tokenSrc.Cancel();

		if (AvatarData != null)
		{
			foreach (var slot in Slots)
			{
				int itemId = AvatarData.Loadout.GetIdForSlot(slot.slot);
				string url = $"{GameManager.Instance.WebsiteName}/items/{itemId}.png";

				GameManager.Instance.CosmeticRetriever.DecrementTextureReference(url);
			}
		}
	}

	void FixedUpdate()
	{
		if (!isInitialized) return;

		Vector3 velocity = ai.Velocity.normalized;
		if (ai.Velocity.magnitude < lookDirChangeThreshold)
			velocity = Vector3.zero;

		if (velocity.z > 0.707f)
			FacingDirection = FacingDir.FRONT;

		if (velocity.z < -0.707f)
			FacingDirection = FacingDir.BACK;

		if (velocity.x > 0.707f)
			FacingDirection = FacingDir.LEFT;

		if (velocity.x < -0.707f)
			FacingDirection = FacingDir.RIGHT;

		foreach (var renderer in cosmeticSlots.Select(s => s.renderer))
		{
			renderer.flipX = FacingDirection == FacingDir.RIGHT;
		}

		float vel = ai.Velocity.magnitude;
		animator.SetFloat("Velocity", vel);

		foreach (var slot in cosmeticSlots)
		{
			int itemId = AvatarData.Loadout.GetIdForSlot(slot.slot);
			string url = $"{GameManager.Instance.WebsiteName}/items/{itemId}.png";

			UpdateRenderer(url, slot.renderer);
		}

		UpdateRenderer($"{GameManager.Instance.WebsiteName}/items/eyes.png", eyesRenderer);
	}

	void Update()
	{
		catchphraseCanvas.localScale = new Vector3(
			transform.position.x >= middleRoomX ? -1 : 1,
			1,
			1
		);

		catchphraseText.transform.localScale = new Vector3(
			transform.position.x <= middleRoomX ? 1 : -1,
			1,
			1
		);

	}

	public void Retire()
	{
		ai.Leave();
	}

	public void TriggerCatchphrase()
	{
		speechBubbleAnimator.Play("Speak");
		Invoke(nameof(TriggerCatchphrase), UnityEngine.Random.Range(minCatchphraseTime, maxCatchphraseTime));
	}

	public async Task Setup(int userId)
	{
		UserId = userId;

		// Hide until images have loaded
		SetRenderersEnabled(false);

		await RefreshData();

		SetRenderersEnabled(true);

		RefreshLoop(tokenSrc.Token);

		isInitialized = true;

		ai.Enter();
	}

	public async Task RefreshData()
	{
		var http_client = new HttpClient(new JSONSerializationOption());

		// Profile Data
		var url = $"{GameManager.Instance.WebsiteName}/UserApi/GetProfile?id={UserId}";
		ProfileData = await http_client.Get<ProfileData>(url);
		catchphraseText.text = ProfileData.Catchphrase;

		// Cosmetic Data
		url = $"{GameManager.Instance.WebsiteName}/UserApi/GetAvatar?id={UserId}";
		AvatarData unappliedAvatarData = await http_client.Get<AvatarData>(url);

		// Cache textures
		List<Task> cacheTasks = new();
		foreach (var slot in Slots)
		{
			int itemId = unappliedAvatarData.Loadout.GetIdForSlot(slot.slot);
			url = $"{GameManager.Instance.WebsiteName}/items/{itemId}.png";

			cacheTasks.Add(GameManager.Instance.CosmeticRetriever.CacheTexture(url));
		}
		// Cache eyes
		cacheTasks.Add(GameManager.Instance.CosmeticRetriever.CacheTexture($"{GameManager.Instance.WebsiteName}/items/eyes.png"));

		await Task.WhenAll(cacheTasks);

		// Increment references for new cosmetics
		foreach (var slot in Slots)
		{
			int itemId = unappliedAvatarData.Loadout.GetIdForSlot(slot.slot);
			url = $"{GameManager.Instance.WebsiteName}/items/{itemId}.png";

			GameManager.Instance.CosmeticRetriever.IncrementTextureReference(url);
		}
		GameManager.Instance.CosmeticRetriever.IncrementTextureReference($"{GameManager.Instance.WebsiteName}/items/eyes.png");

		// Decrement references for old cosmetics
		if (AvatarData != null)
		{
			foreach (var slot in Slots)
			{
				int itemId = AvatarData.Loadout.GetIdForSlot(slot.slot);
				url = $"{GameManager.Instance.WebsiteName}/items/{itemId}.png";

				GameManager.Instance.CosmeticRetriever.DecrementTextureReference(url);
			}
		}
		GameManager.Instance.CosmeticRetriever.IncrementTextureReference($"{GameManager.Instance.WebsiteName}/items/eyes.png");

		AvatarData = unappliedAvatarData;

		SlotData baseSlot = Slots.FirstOrDefault(s => s.slot == CosmeticSlot.BASE);
		if (!baseSlot.Equals(default))
		{
			if (ColorUtility.TryParseHtmlString(AvatarData.SkinColor, out Color color))
			{
				baseSlot.renderer.color = color;
			}
			else
			{
				baseSlot.renderer.color = Color.green;
			}
		}
	}

	// -- Uses string instead of enum because changing the enum breaks animations. --
	// Options for animation names are the names of PublicAnimationEnum:
	// - WALK
	// - IDLE
	public void SetCurrentAnimation(string animation)
	{
		if (Enum.TryParse(animation.ToUpper(), out PublicAnimationEnum result))
		{
			currentAnimation = (AnimationEnum)result;
		}
	}

	async void RefreshLoop(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			await RefreshData();

			await Task.Delay((int)(profileRefreshPeriod * 1000));
		}
	}

	void SetRenderersEnabled(bool enabled)
	{
		foreach (var renderer in cosmeticSlots.Select(s => s.renderer))
		{
			renderer.enabled = enabled;
		}
	}

	void UpdateRenderer(string url, SpriteRenderer renderer)
	{
		int x = currentFrame;
		int y = (int)currentAnimation;

		switch (FacingDirection)
		{
			case FacingDir.RIGHT:
				y += 1;
				break;

			case FacingDir.LEFT:
				y += 1;
				break;

			case FacingDir.BACK:
				y += 2;
				break;
		}

		Texture2D texture = GameManager.Instance.CosmeticRetriever.GetCachedTexture(url);

		Rect rect = new(x * spriteWidth, texture.height - (y + 1) * spriteHeight, spriteWidth, spriteHeight);

		Sprite sprite = GameManager.Instance.CosmeticRetriever.GetSprite(
			url,
			rect,
			new Vector2(0.333f, 0.5f),
			8
		);

		renderer.sprite = sprite;
	}

	[Serializable]
	public struct SlotData
	{
		public CosmeticSlot slot;
		public SpriteRenderer renderer;
	}

	// Only expose the front animations, since the offset for
	// side/back is done in code
	public enum PublicAnimationEnum
	{
		WALK = AnimationEnum.WALK_FRONT,
		IDLE = AnimationEnum.IDLE_FRONT,
	}

	public enum FacingDir
	{
		FRONT,
		RIGHT,
		LEFT,
		BACK,
	}

	enum AnimationEnum
	{
		WALK_FRONT,
		WALK_SIDE,
		WALK_BACK,

		IDLE_FRONT,
		IDLE_SIDE,
		IDLE_BACK,
	}
}