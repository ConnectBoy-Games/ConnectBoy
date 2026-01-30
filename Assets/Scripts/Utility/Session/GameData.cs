using System.Threading.Tasks;
using Wagr;


public interface IGameHandler
{
    GameName Type { get; }
    object State { get; }
    Task<GameResult> MakeMoveAsync(string playerId, object move);
}

public class GameResult
{
    public bool Success { get; set; } //Successful move or not
    public string Message { get; set; }
    public object State { get; set; }
    public bool KeepTurn { get; set; } // If true, the player keeps the turn (e.g., Dots & Boxes when box completed)
}

public class XAndOMove { public int val { get; set; } }

public class XAndOState {public string[] Board { get; set; } public string CurrentTurn { get; set; } public string Winner { get; set; } }




