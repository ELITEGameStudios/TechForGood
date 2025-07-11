using System;
using System.Collections;
using System.Net;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;


public class AvatarStats : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(MakeRequests());
    }

    private IEnumerator MakeRequests(){
        //Getting
        var get_request = CreatRequest("https://jsonplaceholder.typicode.com/todos/1");
        yield return get_request.SendWebRequest();
        var deserializedGetData = JsonUtility.FromJson<Todo>(get_request.downloadHandler.text);

        //Posting
        var data_to_post = new PostData(){Hero = "John Wick", PowerLevel = 9001};
        var post_request = CreatRequest("https://reqbin.com/echo/post/json", RequestType.POST, data_to_post);
        yield return post_request.SendWebRequest();
        var deserializedPostData = JsonUtility.FromJson<PostResult>(post_request.downloadHandler.text);

    }

    private UnityWebRequest CreatRequest(string path, RequestType type = RequestType.GET, object data = null){
        
        var request = new UnityWebRequest(path, type.ToString());

        if (data != null){
            var body_raw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            request.uploadHandler = new UploadHandlerRaw(body_raw);
            
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    private void AttachHeader(UnityWebRequest request, string key, string value){
        request.SetRequestHeader(key, value);
    }
}

public enum RequestType{
    GET = 0,
    POST = 1,
    PUT = 2
}

public class Todo{
    public int userId;
    public int id;
    public string title;
    public bool completed;
}

[Serializable]
public class PostData{
    public string Hero;
    public int PowerLevel;
}

public class PostResult{
    public string success { get; set; }
}

