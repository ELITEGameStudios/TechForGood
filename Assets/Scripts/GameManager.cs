using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static You;

public class GameManager : MonoBehaviour
{

	public DataRetrieveState dataState;
	public List<You> yous;
	public GameObject youPrefab;
	public string websiteName;
	public List<Sprite> sprit;



	[Header("For SN Data Retrieval")]
	[SerializeField] private string[] studentNumbers;
	public UnityWebRequest studentNumbersRequest;

	public enum DataRetrieveState
	{
		IDLE,
		GETTING_STUDENT_NUMBERS,
		GETTING_YOU_DATA,
		FINISHED,
		ERROR
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

		if (dataState != DataRetrieveState.IDLE) { return; }
		dataState = DataRetrieveState.GETTING_STUDENT_NUMBERS;

		yous = new();
		studentNumbers = new string[0];

		// Start student number requests
		InvokeRepeating(nameof(InitiateStudentNumberRetrieval), 0, 5f);
	}

	public async void InitiateStudentNumberRetrieval()
	{
		// todo: BUG: This fails if a user joins at the same time as one leaves,
		// the length is the same so it doesn't detect a change
		var url = websiteName + "/UserApi/GetLabUsers";
		Debug.Log("Getting infor?");

		var http_client = new HttpClient(new JSONSerializationOption());
		string[] newStudentNumbers = await http_client.Get<string[]>(url);
		Debug.Log("Got info? " + newStudentNumbers);

		if (studentNumbers.Length > newStudentNumbers.Length)
		{
			studentNumbers = newStudentNumbers;
			Debug.Log("Deleting you.");
			DeleteYouProcess();
		}
		else if (studentNumbers.Length < newStudentNumbers.Length)
		{
			studentNumbers = newStudentNumbers;
			InitiateYouDataRetrieval();
			Debug.Log("Adding yous");
		}
	}

	void DeleteYouProcess()
	{

		for (int i = yous.Count - 1; i >= 0; i--)
		{
			bool retire = true;
			foreach (string id in studentNumbers)
			{
				if (id == yous[i].studentNumber)
				{
					retire = false;
					break;
				}
			}
			if (retire)
			{
				yous[i].Retire();
				yous.RemoveAt(i);
			}
		}
	}

	async void InitiateYouDataRetrieval()
	{
		dataState = DataRetrieveState.GETTING_YOU_DATA;


		for (int i = 0; i < studentNumbers.Length; i++)
		{
			bool exists = false;
			foreach (You oldYou in yous)
			{
				if (oldYou.studentNumber == studentNumbers[i])
				{
					exists = true;
					Debug.Log("You exist");
				}
			}
			if (exists) continue;

			You you = Instantiate(youPrefab, transform).GetComponent<You>();

			// Profile Data
			var url = websiteName + "/UserApi/GetProfile?id=" + studentNumbers[i].ToString();
			var http_client = new HttpClient(new JSONSerializationOption());
			YouWebClass youBaseData = await http_client.Get<YouWebClass>(url);

			// Cosmetic Data
			url = websiteName + "/UserApi/GetAvatar?id=" + studentNumbers[i].ToString();
			YouCosmeticData youCosmeticData = await http_client.Get<YouCosmeticData>(url);
			await RetrieveCosmeticData(youCosmeticData);
			// Texture profileImage = await GetTextureFromWeb( "http://" + websiteName +"/UserApi/GetProfile?id="+studentNumbers[i].ToString());

			you.SetData(youBaseData, youCosmeticData, studentNumbers[i]);
			Debug.Log("Setting your data");
			yous.Add(you);
			// you.SetData(youBaseData, cosmeticBundle);
		}

	}

	public async Task<YouCosmeticData> RetrieveCosmeticData(YouCosmeticData youCosmeticData)
	{
		Debug.Log("Bundling the struct");
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

	void AddLocalYou()
	{
		// dataState = DataRetrieveState.GETTING_YOU_DATA;


		// for (int i = 0; i < studentNumbers.Length; i++)
		// {
		// bool exists = false;
		// foreach (You oldYou in yous)
		// {
		//     if(oldYou.studentNumber == studentNumbers[i]){
		//         exists = true;
		//     }
		// }
		// if(exists) continue;


		You you = Instantiate(youPrefab, transform).GetComponent<You>();
		yous.Add(you);


		// Texture profileImage = await GetTextureFromWeb( "http://" + websiteName +"/UserApi/GetProfile?id="+studentNumbers[i].ToString());
		// CosmeticBundleStruct cosmeticBundle = await GetCosmeticFromWeb(youBaseData.hatCosmetic);
		// you.SetData(youBaseData, cosmeticBundle);
		// }

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

	void Update()
	{

		// switch (dataState)
		// {
		// case DataRetrieveState.GETTING_STUDENT_NUMBERS:
		//     InitiateStudentNumberRetrieval();
		//     break;

		// case DataRetrieveState.GETTING_YOU_DATA:
		//     InitiateStudentNumberRetrieval();
		//     break;
		// }
	}
}
