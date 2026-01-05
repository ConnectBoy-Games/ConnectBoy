using Newtonsoft.Json;
using System;

namespace Wagr
{
    [Serializable]
    public enum GameName : byte
    {
        xando,
        minifootball,
        archery,
        dotsandboxes,
        fourinarow,
        minigolf
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
        public string matchId; 
        public long timestamp; 
    }

    /// <summary> Represents a gaming session both on the server and client</summary>
    [Serializable]
    public class Session
    {
        public string sessionId { get; private set; }
        public string serverUrl { get; private set; }
        public int wager { get; private set; }
        public string hostId { get; private set; }
        public string hostName { get; private set; }
        public string friendId { get; private set; }
        public string friendName { get; private set; }

        private int hostScore;
        private int friendScore;

        public Session()
        {

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
}
