using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Playlist : MonoBehaviour
{
    public List<AudioClip> user_playlist = new List<AudioClip>();
    public List<AudioClip> default_playlist = new List<AudioClip>();
    public TMP_Text song_display_text;
    public AudioSource current_song;
    public bool randomize_song = false;
    public int song_number = 0;
    public GameObject volume_image;
    public bool muted = false;
    public List<Sprite> volume_sprites = new List<Sprite>();
    public List<Sprite> muted_sprites = new List<Sprite>();
    public Slider volume_slider;
    private float previous_volume;
    //public TMP_Text mute_button_text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        previous_volume = current_song.volume;
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

        if (!muted){
            if (current_song.volume < 0.25f){
                volume_image.GetComponent<Image>().sprite = volume_sprites[0];
            }

            else if (current_song.volume < 0.50f){
                volume_image.GetComponent<Image>().sprite = volume_sprites[1];
            }

            else if (current_song.volume < 0.75f){
                volume_image.GetComponent<Image>().sprite = volume_sprites[2];
            }

            else if (current_song.volume <= 1.00f){
                volume_image.GetComponent<Image>().sprite = volume_sprites[3];
            }
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

    public void MuteAndUnmute(){
        //Unmute
        if (muted){
            volume_slider.interactable = true;
            current_song.volume = previous_volume;
            //mute_button_text.text = "Mute";

            if (current_song.volume < 0.25f){
                volume_image.GetComponent<Image>().sprite = volume_sprites[0];
            }

            else if (current_song.volume < 0.50f){
                volume_image.GetComponent<Image>().sprite = volume_sprites[1];
            }

            else if (current_song.volume < 0.75f){
                volume_image.GetComponent<Image>().sprite = volume_sprites[2];
            }

            else if (current_song.volume <= 1.00f){
                volume_image.GetComponent<Image>().sprite = volume_sprites[3];
            }

            muted = false;
        }

        //Mute
        else{
            previous_volume = current_song.volume;

            if (current_song.volume < 0.25f){
                volume_image.GetComponent<Image>().sprite = muted_sprites[0];
            }

            else if (current_song.volume < 0.50f){
                volume_image.GetComponent<Image>().sprite = muted_sprites[1];
            }

            else if (current_song.volume < 0.75f){
                volume_image.GetComponent<Image>().sprite = muted_sprites[2];
            }

            else if (current_song.volume <= 1.00f){
                volume_image.GetComponent<Image>().sprite = muted_sprites[3];
            }

            current_song.volume = 0.0f;
            volume_slider.interactable = false;
            //mute_button_text.text = "Unmute";
            muted = true;
        }
    }
}