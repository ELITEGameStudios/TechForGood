using UnityEngine;
using TMPro;

public class MainUIManager : MonoBehaviour
{
    public TMP_Text nameText, hoursText, yearText, descriptionText;
    public UIState state;
    public static MainUIManager instance {get; private set;}
    public GameObject UI_panel;
    public Vector2 original_position;
    public Vector2 active_position;
    public float timer;
    public float duration = 2.0f;
    public float speed = 3.5f;

    public enum UIState{
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
        if(instance == null){instance = this;}
        else if(instance != this){Destroy(this);}
    }

    void Update()
    {
        if (timer <= duration){
            timer = timer + Time.deltaTime;

            float percentage = timer/duration;

            if (state == UIState.INSPECTING){
                UI_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(original_position, active_position, Mathf.SmoothStep(0, 1, percentage * speed));
            }

            else{
                UI_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(active_position, original_position, Mathf.SmoothStep(0, 1, percentage * speed));
            }
        }

    }

    public void InspectYou(You you){
        // UI_panel.SetActive(true);
        state = UIState.INSPECTING;
        nameText.text = you.name;
        yearText.text = "Year " + you.year;
        hoursText.text = ((int)you.hoursPlayed).ToString() + " Hours";
        descriptionText.text = you.bio;
        timer = 0.0f;
    }

    public void UnInspect(){
        state = UIState.DEFAULT;
        timer = 0.0f;
        // UI_panel.SetActive(false);
    }
}
