using System;

[Serializable]
public class NotificationObject
{
    public int uid;
    public int userId;
    public int wager;
    public Wagr.GameName gameName;
    public string message;
    public string userName;
    public string timeText;

    public NotificationObject(string Json)
    {

    }
}

[Serializable]
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

[Serializable]
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

[Serializable]
public enum LoginState : byte
{
    unsignned = 0,
    guestMode,
    loggedIn
}

[Serializable]
public enum GameMode : byte
{
    vsBot,
    vsPlayer
}

[Serializable]
public enum User : byte 
{
    bot,
    host,
    player //Other player
}

[Serializable]
public enum BotDifficulty : byte 
{
    low,
    medium,
    high
}
