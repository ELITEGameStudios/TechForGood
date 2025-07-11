public class YouWebClass
{

    // General Information
    public string studentNumber;
    public string name;
    public int year;
    public string nameTag, bio;
    public GameDisplay[] contributedGames;

    public struct GameDisplay{
        public string gameName;
        // public Image gameImage;
        public string gameDescription;
        public int gameYear;
        public bool showGameInYouniverse;
    }

    // Time stats
    public float secondsPlayed;
    public float minutesPlayed {get {return secondsPlayed / 60; }}
    public float hoursPlayed {get {return minutesPlayed / 60; }}

}
