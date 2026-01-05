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

    public PlayerStats()
    {
        gamesPlayed = 0;
        gamesWon = 0;
        gamesLost = 0;
        winRate = 0;
        winStreak = 0;
    }

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
    public string id { get; private set; } //Matches the Unity Player ID
    public string displayName { get; private set; }
    public int dpIndex { get; private set; }
    public int balance { get; private set; }

    public PlayerStats playerStats { private set; get; }

    public Profile(string id, string username)
    {
        this.id = id;
        this.displayName = username;
        this.dpIndex = 0;
        this.balance = 0;
        this.playerStats = new PlayerStats();
    }

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

[System.Serializable]
public class NameResponse
{
    public bool success;
    public string name;
}

// Response structure matches the return object from JS
[System.Serializable]
public class LookupResponse
{
    public bool exists;
    public string playerId;
}

[System.Serializable]
public class LookupRequest
{
    public string targetName;
}
