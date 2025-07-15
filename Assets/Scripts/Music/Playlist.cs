using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Playlist : MonoBehaviour
{
    public List<AudioClip> user_playlist = new List<AudioClip>();
    public List<AudioClip> default_playlist = new List<AudioClip>();
    public TMP_Text song_display_text;
    public AudioSource current_song;
    public bool randomize_song = false;
    public int song_number = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current_song.clip = default_playlist[0];
        current_song.Play();
        song_display_text.text = ("Currently Playing: " + current_song.clip.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (!current_song.isPlaying){
            if (randomize_song){
                RandomSong();
            }

            else{
                NextSong();
            }
            song_display_text.text = ("Currently Playing: " + current_song.clip.name);
        }
    }

    void NextSong(){
        if (song_number < (default_playlist.Count - 1)){
            //In order
            current_song.clip = default_playlist[song_number];
            current_song.Play();
            song_number += 1;
        }
    }
    void RandomSong(){
        int random_song_number = UnityEngine.Random.Range(0, default_playlist.Count);

        while (song_number == random_song_number){
            random_song_number = UnityEngine.Random.Range(0, default_playlist.Count);
        }

        current_song.clip = default_playlist[random_song_number];
        current_song.Play();

    }
}