using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEditor;
using Unity.VisualScripting;

public class HttpClient
{
    private readonly IserializationOption _serialization_option;
    public HttpClient(IserializationOption serialization_option){
        _serialization_option = serialization_option;
    }

    public async Task<TResultType> Get<TResultType>(string url){
        try{
            
            using var www = UnityWebRequest.Get(url);
            www.SetRequestHeader("Content-Type", _serialization_option.content_type);

            var operation = www.SendWebRequest();

            while(!operation.isDone){
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success){
                Debug.LogError($"Failed: {www.error}");
                return default;
            }
            else{

                var result = _serialization_option.Deserialize<TResultType>(www.downloadHandler.text);
                Debug.Log($"Success: {www.downloadHandler.text}");
                return result;
            }   
        }

        catch(Exception ex){
            Debug.LogError($"{nameof(Get)} failed: {ex.Message}");
            return default;
        }
    }
}
