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
    public bool local;



    [Header("For SN Data Retrieval")]
    [SerializeField] private string[] studentNumbers;
    public UnityWebRequest studentNumbersRequest;

    public enum DataRetrieveState{
        IDLE,
        GETTING_STUDENT_NUMBERS,
        GETTING_YOU_DATA,
        FINISHED,
        ERROR
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        if(dataState != DataRetrieveState.IDLE){return;}
        dataState = DataRetrieveState.GETTING_STUDENT_NUMBERS;
        
        yous = new();
        studentNumbers = new string[0];

        // Start student number requests
        InvokeRepeating(nameof(InitiateStudentNumberRetrieval), 0, 5f);
    }

    public async void InitiateStudentNumberRetrieval(){

        var url = websiteName +"/UserApi/GetLabUsers";
        Debug.Log("Getting infor?");

        var http_client = new HttpClient(new JSONSerializationOption());
        string[] newStudentNumbers = await http_client.Get<string[]>(url);
        Debug.Log("Got info?");
        
        if (studentNumbers.Length > newStudentNumbers.Length){
            studentNumbers = newStudentNumbers;
            Debug.Log("Deleting you.");
            if (!local) DeleteYouProcess();
        }
        else if (studentNumbers.Length < newStudentNumbers.Length){
            studentNumbers = newStudentNumbers;
            if (!local) InitiateYouDataRetrieval();
            Debug.Log("Adding yous");
        }
    }

    void DeleteYouProcess(){

        for (int i = yous.Count-1; i >= 0 ; i--)
        {
            bool retire = false;
            foreach (string id in studentNumbers)
            {
                if(id == yous[i].studentNumber){
                    retire = true;
                    break;
                }
            }
            if(retire){
                yous[i].Retire();
                yous.RemoveAt(i);
            }
            
        }
    }

    async void InitiateYouDataRetrieval(){
        dataState = DataRetrieveState.GETTING_YOU_DATA;
        

        for (int i = 0; i < studentNumbers.Length; i++)
        {
            bool exists = false;
            foreach (You oldYou in yous)
            {
                if(oldYou.studentNumber == studentNumbers[i]){
                    exists = true;
                }
            }
            if(exists) continue;


            You you = Instantiate(youPrefab, transform).GetComponent<You>();

            var url = websiteName +"/UserApi/GetAvatar?id="+studentNumbers[i].ToString();
            var http_client = new HttpClient(new JSONSerializationOption());
            YouWebClass youBaseData = await http_client.Get<YouWebClass>(url);

            // Texture profileImage = await GetTextureFromWeb( "http://" + websiteName +"/UserApi/GetProfile?id="+studentNumbers[i].ToString());
            // CosmeticBundleStruct cosmeticBundle = await GetCosmeticFromWeb(youBaseData.hatCosmetic);

            you.SetData(youBaseData);
            yous.Add(you);
            // you.SetData(youBaseData, cosmeticBundle);
        }

    }

    void AddLocalYou(){
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

    public async Task<CosmeticBundleStruct> GetCosmeticFromWeb(int itemId){
        CosmeticBundleStruct cosmeticBundle;
        
        cosmeticBundle.front = await GetTextureFromWeb( websiteName +"/items/" + itemId.ToString() + "_front.png");
        cosmeticBundle.side = await GetTextureFromWeb( websiteName +"/items/" + itemId.ToString() + "_side.png");
        cosmeticBundle.back = await GetTextureFromWeb( websiteName +"/items/" + itemId.ToString() + "_back.png");
        
        return cosmeticBundle;
    }

    public async Task<Texture> GetTextureFromWeb(string url){
        UnityWebRequest www = UnityWebRequestTexture.GetTexture( websiteName +"/items/" + "[ItemCode]".ToString() + "");
        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            return null;
        }
        else {
            return DownloadHandlerTexture.GetContent(www);
        }
    }
    void FinishRetrieval(){
        // Initiate intros and any startupp sequences for the program
        dataState = DataRetrieveState.FINISHED;


    }

    void Update(){

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
