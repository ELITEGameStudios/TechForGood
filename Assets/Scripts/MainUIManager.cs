using UnityEngine;
using TMPro;

public class MainUIManager : MonoBehaviour
{
    public TMP_Text nameText, hoursText;
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
        hoursText.text = ((int)you.hoursPlayed).ToString();
    }
}
