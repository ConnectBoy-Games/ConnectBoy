using System.Collections.Generic;
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
    public string CurrentTurn { get; set; }
    public object State { get; set; }
    public bool KeepTurn { get; set; } // If true, the player keeps the turn (e.g., Dots & Boxes when box completed)
}

//Game States
public class XAndOState
{
    public string[] Board { get; set; }
    public string Winner { get; set; }
}

public class DotsAndBoxesState
{
    public List<int> HorizontalEdges { get; set; } // horizontal edges
    public List<int> VerticalEdges { get; set; } // vertical edges
    public List<int> Boxes { get; set; } // owner per box
    public int Player1Scores { get; set; }
    public int Player2Scores { get; set; }

}

public class FourInARowState
{
    public int[,] Board { get; set; }
    public string Winner { get; set; }
}

public class MiniGolfGameState
{
    public bool isGameOver { get; set; } //Has the game finished?
    public string CurrentTurn { get; set; } //ID of who has the current turn after the previous move
    public string Winner { get; set; } //ID of the current winner, empty if no winner yet or tie

    // The position of the ball after the shot is finished
    public float BallPosX { get; set; }
    public float BallPosY { get; set; }

    public int stateHash { get; set; }// Checksum to verify both players calculated the same result
}

public class MiniBallState
{
    public MiniBallEntity[] entities { get; set; } // Positions of all 6 pieces + ball
    public string CurrentTurn { get; set; }
    public string Winner { get; set; }
}

public class ArcheryState { }


//Game Moves
public class XAndOMove
{
    public int val { get; set; }
}

public class DaBMove
{
    public int H { get; set; }
    public int V { get; set; }
}


public class FourInARowMove
{
    public int col { get; set; }
}

public class MiniGolfMove
{
    public float X { get; set; }
    public float Y { get; set; }
}
public class MiniBallMove
{
    public MiniBallPiece PieceId { get; set; } // Which piece was kicked
    public float forceX { get; set; }
    public float forceY { get; set; }
}


public class MiniBallEntity
{
    public static float radius;
    public static float mass;

    public MiniBallPiece Piece { get; set; } //The type of piece e.g., "Player1_Piece1", "Ball"
    public float PosX { get; set; } //Current X position of the piece
    public float PosY { get; set; } //Current Y position of the piece
    public float velX { get; set; } //Current X velocity of the piece
    public float velY { get; set; } //Current Y velocity of the piece
}

public enum MiniBallPiece : byte
{
    Ball = 0,

    Player1_Piece1 = 1,
    Player1_Piece2 = 2,
    Player1_Piece3 = 3,
    Player1_Piece4 = 4,
    Player1_Piece5 = 5,

    Player2_Piece1 = 6,
    Player2_Piece2 = 7,
    Player2_Piece3 = 8,
    Player2_Piece4 = 9,
    Player2_Piece5 = 10,
}

public class MatchReset
{
    public string matchId { get; set; }
    public int newTurnNumber { get; set; }
    public MiniBallEntity[] entities { get; set; } // The starting positions for all 7 items
}



