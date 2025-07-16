using Newtonsoft.Json;
using UnityEngine;
using System;

public class JSONSerializationOption : IserializationOption
{
    public string content_type => "application/json";

    public T Deserialize<T>(string text){
        try{
            var result = JsonConvert.DeserializeObject<T>(text);
            return result;
        }

        catch(Exception ex){
            Debug.LogError($"Could not parse response {text}. {ex.Message}");
            return default;
        }
    }
}
