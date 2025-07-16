using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static You;

public class GameManager : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] string websiteName;

	[Tooltip("Refresh delay in seconds")]
	[SerializeField] float refreshDelay = 1;

	[Header("References")]
	[SerializeField] GameObject youPrefab;
	[SerializeField] Transform youSpawnPoint;

	CancellationTokenSource refreshSource;
	Dictionary<int, You> yousInLab;

	public enum DataRetrieveState
	{
		IDLE,
		GETTING_STUDENT_NUMBERS,
		GETTING_YOU_DATA,
		FINISHED,
		ERROR
	}

	void Start()
	{
		refreshSource = new();
		yousInLab = new();

		// Start student number requests (loop)
		GetLabStudentsLoop(refreshSource.Token);
	}

	void OnDestroy()
	{
		// Cancel refresh loop
		refreshSource.Cancel();
	}

	public async void GetLabStudentsLoop(CancellationToken token)
	{
		var http_client = new HttpClient(new JSONSerializationOption());

		while (!token.IsCancellationRequested)
		{
			var url = websiteName + "/UserApi/GetLabUsers";

			int[] newStudentNumbers = await http_client.Get<int[]>(url);

			List<Task> tasks = new();
			foreach (var number in newStudentNumbers)
			{
				if (yousInLab.ContainsKey(number))
					continue;

				// New user
				tasks.Add(InitiateYouDataRetrieval(number));
			}

			List<int> usersToDelete = new(); // Required because we can't delete it inside a foreach
			foreach (var number in yousInLab.Keys)
			{
				if (newStudentNumbers.Contains(number))
					continue;

				// User left
				usersToDelete.Add(number);
			}

			foreach (var number in usersToDelete)
			{
				RetireYou(number);
			}

			// Wait for all user data asynchronously
			await Task.WhenAll(tasks);

			// Delay
			await Task.Delay((int)(refreshDelay * 1000));
		}
	}

	void RetireYou(int userId)
	{
		Debug.Log($"Delete You: {userId}");

		You you = yousInLab[userId];

		if (you == null)
			return;

		yousInLab.Remove(userId);
		you.Retire();
	}

	async Task InitiateYouDataRetrieval(int userId)
	{
		Debug.Log($"New You: {userId}");

		var http_client = new HttpClient(new JSONSerializationOption());

		// Profile Data
		var url = websiteName + $"/UserApi/GetProfile?id={userId}";
		YouWebClass youBaseData = await http_client.Get<YouWebClass>(url);

		// Cosmetic Data
		url = websiteName + $"/UserApi/GetAvatar?id={userId}";
		YouCosmeticData youCosmeticData = await http_client.Get<YouCosmeticData>(url);
		await RetrieveCosmeticData(youCosmeticData);

		You you = Instantiate(youPrefab, youSpawnPoint).GetComponent<You>();
		you.transform.localPosition = Vector3.zero;

		yousInLab.Add(userId, you);

		you.SetData(youBaseData, youCosmeticData, userId);
	}

	public async Task<YouCosmeticData> RetrieveCosmeticData(YouCosmeticData youCosmeticData)
	{
		List<Task> allTasks = new()
		{
			// Base
			GetCosmeticFromWeb(youCosmeticData.Loadout.BaseItemId)
				.ContinueWith(u => youCosmeticData.baseData = u.Result),

			// Shirt
			GetCosmeticFromWeb(youCosmeticData.Loadout.ShirtItemId)
				.ContinueWith(u => youCosmeticData.shirtData = u.Result),
		};

		// Wait for all tasks asynchronously
		await Task.WhenAll(allTasks);

		return youCosmeticData;
	}

	public async Task<CosmeticBundleClass> GetCosmeticFromWeb(int itemId)
	{
		Texture texture = await GetTextureFromWeb(websiteName + "/items/" + itemId.ToString() + ".png");
		if (texture == null) { return null; }
		return new CosmeticBundleClass(texture);
	}

	public async Task<Texture> GetTextureFromWeb(string url)
	{
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
		await www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			return null;
		}
		else
		{
			return DownloadHandlerTexture.GetContent(www);
		}
	}
}
