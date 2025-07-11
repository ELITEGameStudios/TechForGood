using UnityEngine;
using UnityEngine.UI;

public class You : MonoBehaviour
{



    // General Information
    public string studentNumber;
    public string name;
    public string year;
    public string nameTag, bio;
    public GameDisplay[] contributedGames;

    // Visual Data

    public CosmeticBundleStruct cosmeticBundle;
    public Texture profileImage;

    public struct GameDisplay{
        public string gameName;
        public Image gameImage;
        public string gameDescription;
        public int gameYear;
        public bool showGameInYouniverse;
    }

    public struct CosmeticBundleStruct {
        public Texture front;
        public Texture side;
        public Texture back;
        public bool complete {
            get{
                return front != null 
                && side != null 
                && back != null;
            }
        }
    }

    // Time stats
    public float secondsPlayed;
    public float minutesPlayed {get {return secondsPlayed / 60; }}
    public float hoursPlayed {get {return minutesPlayed / 60; }}

    public void SetData(YouWebClass youWebClass){ // , CosmeticBundleStruct cosmetics){
        name = youWebClass.name;
        studentNumber = youWebClass.studentNumber;
        year = youWebClass.year;
        nameTag = youWebClass.nameTag;
        bio = youWebClass.bio;
        // cosmeticBundle = cosmetics;
        // this.profileImage = profileImage;
    }

    public void Retire(){}



}
