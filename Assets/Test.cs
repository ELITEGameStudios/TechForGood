// Loads all assets in the "Resources/Textures" folder
// Then picks a random one from the list.
// Note: Random.Range in this case returns [low,high]
// range, i.e. the high value is not included in the range.
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    private AudioClip[] game_songs;
    private AudioClip audiooo;
    private GameObject go;

    void Start()
    {

        game_songs = Resources.LoadAll<AudioClip>("Sound/Game Music");

        foreach (var t in game_songs)
        {
            Debug.Log(t.name);
        }


    }

    // void OnGUI()
    // {
    //     if (GUI.Button(new Rect(10, 70, 150, 30), "Change texture"))
    //     {
    //         // change texture on cube
    //         Texture2D texture = (Texture2D)textures[Random.Range(0, textures.Length)];
    //         go.GetComponent<Renderer>().material.mainTexture = texture;
    //     }
    // }
}