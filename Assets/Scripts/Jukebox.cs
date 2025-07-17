using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using TMPro;

public class Jukebox : MonoBehaviour
{
    public Animator lid_animator;
    public Animator record_animator;
    public Animator needle_animator;
    private bool opened = false;
    public static JukeboxUIState jukebox_state;
    public GameObject jukebox_UI_panel;
	public Vector2 original_position;
	public Vector2 active_position;
	public float timer;
	public float duration = 2.0f;
	public float speed = 3.5f;
	public AudioClip enter_sound;
	public AudioClip exit_sound;
	public AudioSource SFX_player;
    public GameObject music_button;
    public GameObject grid_layout;


    private AudioClip[] game_songs;

    public enum JukeboxUIState
	{
		DEFAULT,
		INSPECTING
	}

    void Awake()
    {
        game_songs = Resources.LoadAll<AudioClip>("Sound/Game Music");

        for (int i = 0; i < game_songs.Length; i++){
            GameObject button = Instantiate(music_button, grid_layout.transform);
            button.GetComponentInChildren<TMP_Text>().text = game_songs[i].name;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original_position = jukebox_UI_panel.GetComponent<RectTransform>().anchoredPosition;
		timer = duration;
        record_animator.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= duration)
		{
			timer += Time.deltaTime;

			float percentage = timer / duration;

			if (jukebox_state == JukeboxUIState.INSPECTING)
			{
				jukebox_UI_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(original_position, active_position, Mathf.SmoothStep(0, 1, percentage * speed));
				//DOFVolume.weight = percentage * speed;
			}

			else
			{
				jukebox_UI_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(active_position, original_position, Mathf.SmoothStep(0, 1, percentage * speed));
				//DOFVolume.weight = 1 - (percentage * speed);
			}
		}
    }

    private void OnMouseDown()
    {
        if (!opened){
            CameraPositionControl.instance.MoveCamToPos(new Vector3 (gameObject.transform.position.x + 1.0f, gameObject.transform.position.y, gameObject.transform.position.z), 0.5f, 5.0f);
            opened = true;
            lid_animator.SetTrigger("Open");
            needle_animator.SetTrigger("Play");
            record_animator.speed = 1;
            jukebox_state = JukeboxUIState.INSPECTING;
            timer = 0.0f;
        }
        
        else{
            CameraPositionControl.instance.MoveCamToPos(CameraPositionControl.instance.defaultPos, 0.5f);
            opened = false;
            lid_animator.SetTrigger("Close");
            needle_animator.SetTrigger("UnPlay");
            record_animator.speed = 0;
            jukebox_state = JukeboxUIState.DEFAULT;
            timer = 0.0f;
        }
    }
}
