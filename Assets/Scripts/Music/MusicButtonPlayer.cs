using UnityEngine;

public class MusicButtonPlayer : MonoBehaviour
{
    private AudioSource audio_source;
    public AudioClip button_song;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio_source = GameObject.FindGameObjectWithTag("PlaylistManager").GetComponent<AudioSource>();
    }

    public void ChangeSong(){
        audio_source.Stop();
        Invoke("PlaySong", 1.0f);
    }

    void PlaySong(){
        audio_source.clip = button_song;
        audio_source.Play();
    }
}
