using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEditor.ShaderGraph.Internal;

public class MainUIManager : MonoBehaviour
{
	public TMP_Text nameText, hoursText, yearText, descriptionText, pronounsText;
	public GameObject image_photo;
	public static UIState state;
	public static MainUIManager instance { get; private set; }
	public GameObject UI_panel;
	public GameObject UI_panel_bars;
	public Vector2 original_position;
	public Vector2 active_position;
	public float timer;
	public float duration = 2.0f;
	public float speed = 3.5f;
	public AudioClip enter_sound;
	public AudioClip exit_sound;
	public AudioSource SFX_player;
	[SerializeField] private Volume DOFVolume;
	private string prefix;

	public enum UIState
	{
		DEFAULT,
		INSPECTING
	}

	void Start()
	{
		original_position = UI_panel.GetComponent<RectTransform>().anchoredPosition;
		timer = duration;
	}
	void Awake()
	{
		if (instance == null) { instance = this; }
		else if (instance != this) { Destroy(this); }
	}

	void Update()
	{
		if (timer <= duration)
		{
			timer += Time.deltaTime;

			float percentage = timer / duration;

			if (state == UIState.INSPECTING)
			{
				UI_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(original_position, active_position, Mathf.SmoothStep(0, 1, percentage * speed));
				DOFVolume.weight = percentage * speed;
			}

			else
			{
				UI_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(active_position, original_position, Mathf.SmoothStep(0, 1, percentage * speed));
				DOFVolume.weight = 1 - (percentage * speed);
			}
		}

	}

	public void InspectYou(You you)
	{
		ProfileData profileData = you.ProfileData;

		// UI_panel.SetActive(true);
		SFX_player.clip = enter_sound;
		SFX_player.Play();
		state = UIState.INSPECTING;
		Color main_color = Color.blue;
		Color secondary_color = Color.red;
		
		if (ColorUtility.TryParseHtmlString(profileData.PrimaryColor, out main_color)){
			UI_panel.GetComponent<Image>().color = main_color;
		}

		if (ColorUtility.TryParseHtmlString(profileData.SecondaryColor, out secondary_color)){
			UI_panel_bars.GetComponent<Image>().color = secondary_color;
		}

		nameText.text = $"{profileData.FirstName} {profileData.LastName}";
		pronounsText.text = "Pronouns: " + profileData.Pronouns;
		yearText.text = "Year " + profileData.Year;
		hoursText.text = ((int)you.ProfileData.Hours).ToString() + " Hours Logged In";

		prefix = "a ";
		
		if (profileData.Role[0] == 'a' || profileData.Role[0] == 'e' || profileData.Role[0] == 'i' || profileData.Role[0] == 'o' || profileData.Role[0] == 'u'){
			prefix = "an ";
		}

		if (profileData.Role == "Other"){
			prefix = "";
		}

		descriptionText.text = "I'm a team member of " + profileData.GDWTeam + ", and my role on the team is " + prefix + profileData.Role + ". I like to say " + profileData.Catchphrase + " a lot.";
		timer = 0.0f;
	}

	public void UnInspect()
	{
		SFX_player.clip = exit_sound;
		SFX_player.Play();
		state = UIState.DEFAULT;
		timer = 0.0f;
		// UI_panel.SetActive(false);
	}
}
