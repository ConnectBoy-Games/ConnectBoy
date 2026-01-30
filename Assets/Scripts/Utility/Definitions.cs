using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class PlayerStats
{
    public uint gamesPlayed { get; private set; }
    public uint gamesWon { get; private set; }
    public uint gamesLost { get; private set; }
    public uint winRate { get; private set; }
    public uint winStreak { get; private set; }

    [JsonConstructor]
    public  PlayerStats(uint gamesPlayed, uint gamesWon, uint gamesLost, uint winRate, uint winStreak)
    {
        this.gamesPlayed = gamesPlayed;
        this.gamesWon = gamesWon;
        this.gamesLost = gamesLost;
        this.winRate = winRate;
        this.winStreak = winStreak;
    }

    public PlayerStats()
    {
        gamesPlayed = 0;
        gamesWon = 0;
        gamesLost = 0;
        winRate = 0;
        winStreak = 0;
    }

    public PlayerStats(object json)
    {
        Dictionary<string, object> _object = (Dictionary<string, object>)json;
        gamesPlayed = Convert.ToUInt32(_object["gamesPlayed"]);
        gamesWon = Convert.ToUInt32(_object["gamesWon"]);
        gamesLost = Convert.ToUInt32(_object["gamesLost"]);
        winRate = Convert.ToUInt32(_object["winRate"]);
        winStreak = Convert.ToUInt32(_object["winStreak"]);
    }
}

[Serializable]
public class Profile
{
    public string id { get; private set; } //Matches the Unity Player ID
    public string displayName { get; private set; }
    public int dpIndex { get; private set; }
    public int balance { get; private set; }

    [JsonConstructor]
    public Profile(string id, string displayName, int dpIndex = 0, int balance = 0) 
    {
        this.id = id;
        this.displayName = displayName;
        this.dpIndex = dpIndex;
        this.balance = balance;
    }

    public Profile(string id, string username)
    {
        this.id = id;
        this.displayName = username;
        this.dpIndex = 0;
        this.balance = 0;
    }

    public Profile(object json)
    {
        Dictionary<string, object> _object = (Dictionary<string, object>)json;

        id = _object["id"].ToString();
        displayName = _object["displayName"].ToString();
        dpIndex = Convert.ToInt32(_object["dpIndex"]);
        balance = Convert.ToInt32(_object["balance"]);
    }
}

[Serializable]
public enum LoginState : byte
{
    unsigned = 0, guestMode, loggedIn
}

[Serializable]
public enum GameMode : byte
{
    vsBot = 0, vsPlayer = 1
}

[Serializable]
public enum GameRole : byte
{
    /// <summary>This client is the game host</summary>
    host = 0, 
    /// <summary>This client is not the game host</summary>
    friend = 1
}

[Serializable]
public enum User : byte 
{
    bot = 0, host = 1, player = 2 //Other player
}

[Serializable]
public enum BotDifficulty : byte 
{
    low, medium, high
}

[Serializable]
public class NameResponse
{
    public bool success;
    public string name;
}

[Serializable]
public class LookupResponse
{
    public bool exists;
    public string playerId;
}

[Serializable]
public class LookupRequest
{
    public string targetName;
}

[Serializable]
public class CloudCodeResponse
{
    public bool success;
    public string access;
}

[Serializable]
public class CloudProfileGetProxy
{
    public Wagr.Player data;
    public bool success;
}

[Serializable]
public class CloudInvitesGetProxy
{
    public bool success;
    public object data;
}