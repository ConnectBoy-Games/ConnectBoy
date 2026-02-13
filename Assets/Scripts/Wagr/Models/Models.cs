using System;
using Newtonsoft.Json;

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

    /// <summary>Represents a player in the game and on the server</summary>
    [Serializable]
    public class Player
    {
        /// <summary>The player's Unity ID</summary>
        public string Id { get; set; }

        /// <summary>The player's display name</summary>
        public string Name { get; set; }

        /// <summary>The player's profile image index</summary>
        public int DpIndex { get; set; }

        public Player(string id, string name, int dpIndex = -1)
        {
            Id = id;
            Name = name;
            DpIndex = dpIndex;
        }
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
        public Guid sessionId { get; set; }
        public int wager { get; set; }
        public Player client { get; set; } //Refers to the current client player
        public Player other { get; set; } //Refers to the other player

        public GameName gameName;
        public GameRole gameRole;

        //For creating Server based modes
        public Session(Guid matchId, GameName name, int wager, Player client, Player other, GameRole role)
        {
            sessionId = matchId;
            gameName = name;
            gameRole = role;

            this.wager = wager;
            this.client = client;
            this.other = other;
        }

        //For creating offline game modes 
        public Session(GameName name)
        {
            gameName = name;
        }

        public Session(string json)
        {
            var sesh = JsonConvert.DeserializeObject<Session>(json);

            sessionId = sesh.sessionId;
            wager = sesh.wager;
            client = sesh.client;
            client = sesh.client;
            other = sesh.other;
            other = sesh.other;
        }
    }
}
