using UnityEngine;
using TMPro;

public class MainUIManager : MonoBehaviour
{
    public TMP_Text nameText, hoursText, yearText, descriptionText;
    public UIState state;
    public static MainUIManager instance {get; private set;}

    public enum UIState{
        DEFAULT,
        INSPECTING
    }

    void Awake()
    {
        if(instance == null){instance = this;}
        else if(instance != this){Destroy(this);}
    }

    public void InspectYou(You you){
        state = UIState.INSPECTING;
        nameText.text = you.name;
        yearText.text = "Year " + you.year;
        hoursText.text = ((int)you.hoursPlayed).ToString() + " Hours";
        descriptionText.text = you.bio;
    }
}
