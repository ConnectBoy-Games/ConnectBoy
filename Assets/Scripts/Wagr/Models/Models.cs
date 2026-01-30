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
        public Guid sessionId { get; private set; }
        public int wager { get; private set; }
        public Player host { get; private set; } //Always refers to the player that created the game session
        public Player other { get; private set; } //Refers to the other player

        public GameMode gameMode;
        public GameName gameName;
        public GameRole gameRole;

        public BotDifficulty botDifficulty = BotDifficulty.low;

        //For creating Server based modes
        public Session(Guid matchId, GameName name, int wager, Player host, Player other, GameRole role)
        {
            sessionId = matchId;
            gameName = name;
            gameRole = role;
            gameMode = GameMode.vsPlayer;

            this.wager = wager;
            this.host = host;
            this.other = other;
        }

        //For creating AI game modes 
        public Session(GameName name)
        {
            gameName = name;
            gameMode = GameMode.vsBot;
            gameRole = GameRole.host;
        }

        public Session(string json)
        {
            var sesh = JsonConvert.DeserializeObject<Session>(json);

            sessionId = sesh.sessionId;
            wager = sesh.wager;
            host = sesh.host;
            host = sesh.host;
            other = sesh.other;
            other = sesh.other;
        }
    }
}
