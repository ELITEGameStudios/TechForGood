using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static You;

public class GameManager : MonoBehaviour
{
	static GameManager instance;
	public static GameManager Instance => instance;

	[Header("Settings")]
	[SerializeField] string websiteName;

	[Tooltip("Refresh delay in seconds")]
	[SerializeField] float refreshDelay = 1;

	[Header("References")]
	[SerializeField] GameObject youPrefab;
	[SerializeField] Transform youSpawnPointHACK;
	[SerializeField] Transform youSpawnPoint;

	readonly CancellationTokenSource refreshSource = new();
	readonly Dictionary<int, You> yousInLab = new();
	readonly CosmeticRetriever cosmeticRetriever = new();

	public CosmeticRetriever CosmeticRetriever => cosmeticRetriever;
	public string WebsiteName => websiteName;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
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
				tasks.Add(CreateYou(number));
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
		Destroy(you.gameObject);
	}

	async Task CreateYou(int userId)
	{
		Debug.Log($"New You: {userId}");

		You you = Instantiate(youPrefab, youSpawnPointHACK.transform.position, Quaternion.identity, transform).GetComponent<You>();
		you.transform.position = youSpawnPoint.transform.position;

		yousInLab.Add(userId, you);

		await you.Setup(userId);
	}
}
