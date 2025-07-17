using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class You : MonoBehaviour
{
	const int spriteWidth = 48;
	const int spriteHeight = 48;

	[SerializeField] float profileRefreshPeriod = 5;
	[SerializeField] float lookDirChangeThreshold = 0.5f;

	// Stores which slot is in which index, plus renderer info
	[SerializeField] SlotData[] cosmeticSlots;

	public float secondsPlayed;
	public float minutesPlayed { get { return secondsPlayed / 60; } }
	public float hoursPlayed { get { return minutesPlayed / 60; } }

	readonly CancellationTokenSource tokenSrc = new();
	AvatarAI ai;
	Animator animator;
	FacingDir facingDir;
	AnimationEnum currentAnimation;
	bool isInitialized = false;

	public int currentFrame; // set by animations

	public SlotData[] Slots => cosmeticSlots;
	public int UserId { get; set; }
	public ProfileData ProfileData { get; private set; }
	public AvatarData AvatarData { get; private set; }

	void Awake()
	{
		ai = GetComponent<AvatarAI>();
		animator = GetComponent<Animator>();
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

				GameManager.Instance.CosmeticRetriever.IncrementTextureReference(url);
			}
		}
	}

	void FixedUpdate()
	{
		if (!isInitialized) return;

		secondsPlayed += Time.fixedDeltaTime;

		Vector3 velocity = ai.GetAgent().desiredVelocity.normalized;

		if (velocity.z > lookDirChangeThreshold)
			facingDir = FacingDir.FRONT;

		if (velocity.z < -lookDirChangeThreshold)
			facingDir = FacingDir.BACK;

		if (velocity.x > lookDirChangeThreshold)
			facingDir = FacingDir.LEFT;

		if (velocity.x < -lookDirChangeThreshold)
			facingDir = FacingDir.RIGHT;

		float vel = ai.GetAgent().velocity.magnitude;
		animator.SetFloat("Velocity", vel);

		foreach (var renderer in cosmeticSlots.Select(s => s.renderer))
		{
			renderer.flipX = facingDir == FacingDir.LEFT;
		}

		foreach (var slot in cosmeticSlots)
		{
			int itemId = AvatarData.Loadout.GetIdForSlot(slot.slot);
			int x = currentFrame;
			int y = (int)currentAnimation;
			y += (int)facingDir;

			string url = $"{GameManager.Instance.WebsiteName}/items/{itemId}.png";

			Texture2D texture = GameManager.Instance.CosmeticRetriever.GetCachedTexture(url);

			Rect rect = new(x * spriteWidth, texture.height - (y + 1) * spriteHeight, spriteWidth, spriteHeight);

			Sprite sprite = GameManager.Instance.CosmeticRetriever.GetSprite(
				url,
				rect,
				new Vector2(0.333f, 0.5f),
				8
			);

			slot.renderer.sprite = sprite;
		}
	}

	public async Task Setup(int userId)
	{
		UserId = userId;

		transform.localPosition = Vector3.zero;

		// Hide until images have loaded
		SetRenderersEnabled(false);

		await RefreshData();

		SetRenderersEnabled(true);

		RefreshLoop(tokenSrc.Token);

		isInitialized = true;
	}

	public async Task RefreshData()
	{
		var http_client = new HttpClient(new JSONSerializationOption());

		// Profile Data
		var url = $"{GameManager.Instance.WebsiteName}/UserApi/GetProfile?id={UserId}";
		ProfileData = await http_client.Get<ProfileData>(url);

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

		await Task.WhenAll(cacheTasks);

		// Increment references for new cosmetics
		foreach (var slot in Slots)
		{
			int itemId = unappliedAvatarData.Loadout.GetIdForSlot(slot.slot);
			url = $"{GameManager.Instance.WebsiteName}/items/{itemId}.png";

			GameManager.Instance.CosmeticRetriever.IncrementTextureReference(url);
		}

		// Decrement references for old cosmetics
		if (AvatarData != null)
		{
			foreach (var slot in Slots)
			{
				int itemId = AvatarData.Loadout.GetIdForSlot(slot.slot);
				url = $"{GameManager.Instance.WebsiteName}/items/{itemId}.png";

				GameManager.Instance.CosmeticRetriever.IncrementTextureReference(url);
			}
		}

		AvatarData = unappliedAvatarData;
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

	enum AnimationEnum
	{
		WALK_FRONT,
		WALK_SIDE,
		WALK_BACK,

		IDLE_FRONT,
		IDLE_SIDE,
		IDLE_BACK,
	}

	enum FacingDir
	{
		FRONT = 0,
		RIGHT = 1,
		LEFT = 1,
		BACK = 2,
	}
}