using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class WebsiteData : MonoBehaviour
{
    [ContextMenu("Test Get")]
    public async void TestGet(){


        var url = "https://jsonplaceholder.typicode.com/todos/1";
        var http_client = new HttpClient(new JSONSerializationOption());
        var result = await http_client.Get<User>(url);

        Debug.Log(result.UserId);
        Debug.Log(result.Title);

        // using var www = UnityWebRequest.Get(url);
        // www.SetRequestHeader("Content-Type", "application/json");
        // var operation = www.SendWebRequest();

        // while(!operation.isDone){
        //     await Task.Yield();
        // }

        // var json_response = www.downloadHandler.text;

        // if (www.result != UnityWebRequest.Result.Success){
        //     Debug.LogError($"Failed: {www.error}");
        // }

        // try{
        //     var result = JsonConvert.DeserializeObject<User>(json_response);
        //     Debug.Log($"Success: {www.downloadHandler.text}");
        // }

        // catch(Exception ex){
        //     Debug.LogError($"{this} Could not parse response {json_response}. {ex.Message}");
        // }
    }
}
