using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Wagr
{
    [Serializable]
    public enum GameName : byte
    {
        xando = 0,
        minisoccer = 1,
        archery = 2,
        dotsandboxes = 3,
        fourinarow = 4,
        minigolf = 5
    }

    [Serializable]
    public enum ChatType : byte
    {
        invalid = 0
    }

    [Serializable]
    public enum ActionType : byte
    {
        invalid = 0
    }

    [System.Serializable]
    public class MatchInvite 
    { 
        public string senderId; 
        public string senderUsername; 
        public int matchType;         
        public int wagerAmount;       
        public Guid matchId; 
        public long timestamp; 
    }

    /// <summary> Represents a gaming session both on the server and client</summary>
    [Serializable]
    public class Session
    {
        public Guid sessionId { get; private set; }
        public string serverUrl { get; private set; }
        public int wager { get; private set; }
        public string hostId { get; private set; } //Always refers to the current player
        public string hostName { get; private set; }
        public string friendId { get; private set; } //Refers to the other player
        public string friendName { get; private set; }

        private int hostScore;
        private int friendScore;

        public GameMode gameMode;
        public GameName gameName;
        public BotDifficulty botDifficulty;

        //For creating Server based modes
        public Session(Guid matchId, int wager, string hostId, string friendId)
        {
            this.sessionId = matchId;
            this.wager = wager;
            this.hostId = hostId;
            this.friendId = friendId;
        }
        
        //For creating AI game modes 
        public Session(GameMode mode, GameName name)
        {
            gameMode = mode;
            gameName = name;
        }

        public Session(string json)
        {
            var sesh = JsonConvert.DeserializeObject<Session>(json);

            sessionId = sesh.sessionId;
            serverUrl = sesh.serverUrl;
            wager = sesh.wager;
            hostId = sesh.hostId;
            hostName = sesh.hostName;
            friendId = sesh.friendId;
            friendName = sesh.friendName;
            hostScore = sesh.hostScore;
            friendScore = sesh.friendScore;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    [System.Serializable]
    public class SessionData
    {
        public string sessionId;
        public List<string> participantIds;
    }

}
