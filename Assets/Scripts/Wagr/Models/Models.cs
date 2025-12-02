namespace Wagr
{
    public enum LoginState : byte
    {
        guestMode = 0,
        loggedIn
    }

    public class PlayerStats
    {
        uint gamesPlayed { get; set; }
        uint gamesWon { get; set; }
        uint gamesLost { get; set; }
        uint winRate { get; set; }
        uint winStreak { get; set; }

        public PlayerStats(string json)
        {

        }

        public void ToJson()
        {

        }
    }

    public class Profile
    {
        public int uid { get; private set; }
        public int dpIndex { get; private set; }
        public int balance { get; private set; }
        public string username { get; private set; }

        public Profile(string json)
        {

        }
    }
}
