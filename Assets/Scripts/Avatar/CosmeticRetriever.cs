using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CosmeticRetriever
{
	// Unload textures not accessed for this long (todo:)
	// todo: look into a ref count system instead
	public const float textureTimeout = 60;

	readonly Dictionary<string, TextureEntry> loadedTextures = new();
	readonly Dictionary<SpriteKey, Sprite> loadedSprites = new();

	public async Task CacheTexture(string url)
	{
		if (!loadedTextures.ContainsKey(url))
		{
			await LoadTexture(url);
		}
	}

	public Texture2D GetCachedTexture(string url)
	{
		return loadedTextures[url].Texture2D;
	}

	public void IncrementTextureReference(string url)
	{
		loadedTextures[url].RefCount++;
	}

	public void DecrementTextureReference(string url)
	{
		loadedTextures[url].RefCount--;

		if (loadedTextures[url].RefCount <= 0)
		{
			Texture2D texture = loadedTextures[url].Texture2D;

			List<SpriteKey> removeKeys = new();
			foreach (var kv in loadedSprites)
			{
				if (kv.Value.texture != texture)
					continue;

				removeKeys.Add(kv.Key);
				Object.Destroy(kv.Value);
			}

			foreach (var key in removeKeys)
			{
				loadedSprites.Remove(key);
			}

			Object.Destroy(texture);
			loadedTextures.Remove(url);
		}
	}

	// Get an already loaded sprite, or create a new one if needed
	public Sprite GetSprite(string url, Rect rect, Vector2 pivot, float ppu)
	{
		if (!loadedTextures.ContainsKey(url))
		{
			Debug.LogError("Texture was unloaded, but it's required! Loading again, but this will cause a stall.");
			LoadTexture(url).Wait();
		}

		Texture2D texture = loadedTextures[url].Texture2D;

		SpriteKey key = new()
		{
			Texture = texture,
			Rect = rect,
			Pivot = pivot,
			PPU = ppu,
		};

		if (!loadedSprites.ContainsKey(key))
		{
			Sprite sprite = Sprite.Create(texture, rect, pivot, ppu);
			loadedSprites.Add(key, sprite);
		}

		return loadedSprites[key];
	}

	async Task LoadTexture(string url)
	{
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
		await www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			return;
		}

		Texture2D tex = DownloadHandlerTexture.GetContent(www);
		tex.filterMode = FilterMode.Point;

		TextureEntry entry = new(tex);

		loadedTextures.Add(url, entry);
	}

	class TextureEntry
	{
		public Texture2D Texture2D { get; private set; }
		public int RefCount { get; set; }

		public TextureEntry(Texture2D texture2D)
		{
			Texture2D = texture2D;
			RefCount = 0;
		}
	}

	struct SpriteKey
	{
		public Texture2D Texture { get; set; }
		public Rect Rect { get; set; }
		public Vector2 Pivot { get; set; }
		public float PPU { get; set; }
	}
}