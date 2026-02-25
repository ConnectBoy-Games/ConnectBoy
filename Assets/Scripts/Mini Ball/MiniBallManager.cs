using System.Collections.Generic;
using UnityEngine;

public class MiniBallManager : MonoBehaviour, IGameManager
{
    private MiniBallState localState = new();
    private User turnUser; //Who has the turn
    private MiniBallBot bot; //Reference to the game bot
    private bool isGameOver = false;

    [Header("UI Handling")]
    [SerializeField] private MiniBallUIHandler uiHandler;
    [SerializeField] private Transform playerHolder; //The parent object that holds the player pieces

    [Header("Game Handling")]
    public GoalZone player1Goal; // e.g., Left side
    public GoalZone player2Goal; // e.g., Right side

    private int scoreP1 = 0;
    private int scoreP2 = 0;

    public void ProcessPhysicsFrame(Dictionary<string, Vector2> frameData)
    {
        // 1. Get the ball's position from this specific frame
        if (frameData.TryGetValue("Ball", out Vector2 ballPos))
        {
            // 2. Check if it entered Player 1's goal (means Player 2 scored)
            if (player1Goal.Contains(ballPos))
            {
                OnGoalScored("Player 2");
            }
            // 3. Check if it entered Player 2's goal (means Player 1 scored)
            else if (player2Goal.Contains(ballPos))
            {
                OnGoalScored("Player 1");
            }
        }
    }

    private void OnGoalScored(string winner)
    {
        Debug.Log($"GOAL! {winner} scores!");
        // Stop simulation, trigger UI, and reset the field
    }

    public static MiniBallEntity[] GetKickoffFormation()
    {
        return new MiniBallEntity[]
        {
            // Ball in the center
            new MiniBallEntity { Piece = MiniBallPiece.Ball, PosX = 0, PosY = 0},

            // Player 1 Team A (Left side - Blue)
            new MiniBallEntity { Piece = MiniBallPiece.Player1_Piece1, PosX = -8, PosY = 0, },
            new MiniBallEntity { Piece = MiniBallPiece.Player1_Piece2, PosX = -4, PosY = -2, },
            new MiniBallEntity { Piece = MiniBallPiece.Player1_Piece3, PosX = -2, PosY = 2, },

            // Player 2 Team B (Right side - Red)
            new MiniBallEntity { Piece = MiniBallPiece.Player2_Piece1, PosX = 8, PosY = 0, },
            new MiniBallEntity { Piece = MiniBallPiece.Player2_Piece2, PosX = 4, PosY = 2, },
            new MiniBallEntity { Piece = MiniBallPiece.Player2_Piece3, PosX = 2, PosY = -2, }
        };
    }

    public void ExecuteReset(MatchReset packet)
    {
        // Stop any ongoing physics or animations
        StopAllCoroutines();

        foreach (var data in packet.entities)
        {
            // Find the GameObject representing this piece
            GameObject piece = GameObject.Find(data.Piece.ToString());

            if (piece != null)
            {
                // 1. Move the visual object
                piece.transform.position = new Vector3(data.PosX, data.PosY, 0);

                // 2. Clear any velocities (so they don't start moving immediately)
                data.velX = 0;
                data.velY = 0;
            }
        }

        Debug.Log("Field reset. Ready for next turn.");
    }

    public void SwitchTurns()
    {
        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (turnUser == User.bot) ? User.client : User.bot;
                break;
            case GameMode.vsPlayer:
                turnUser = (turnUser == User.client) ? User.player : User.client;
                break;
            case GameMode.online:
                turnUser = (turnUser == User.client) ? User.player : User.client;
                break;
        }
        uiHandler.SetTurnText(turnUser); //Display the turn text
        SetIndicator();
    }

    private void SetIndicator()
    {
        switch (turnUser)
        {
            case User.bot:
                foreach (Transform piece in playerHolder)
                {
                    piece.GetComponent<PlayerPiece>().SetIndictator(true);
                }
                break;
            case User.client:
                foreach (Transform piece in playerHolder)
                {
                    piece.GetComponent<PlayerPiece>().SetIndictator(true);
                }
                break;
            case User.player:
                foreach (Transform piece in playerHolder)
                {
                    piece.GetComponent<PlayerPiece>().SetIndictator(true);
                }
                break;
        }
    }
    
    /// <summary>Places the player's pieces according to a predefined formation</summary>
    private void PlacePieces(int form)
    {

    }

    public void ClearBoard()
    {
        throw new System.NotImplementedException();
    }

    public void CheckBoardState()
    {
        throw new System.NotImplementedException();
    }

    public int CheckWinState(string piece)
    {
        throw new System.NotImplementedException();
    }

    public void GetGameState()
    {
        throw new System.NotImplementedException();
    }
}

public class GoalZone
{
    public User teamName;
    public float minX, maxX;
    public float minY, maxY;

    // A helper to check if the ball is currently inside this box
    public bool Contains(Vector2 point)
    {
        return point.x >= minX && point.x <= maxX &&
               point.y >= minY && point.y <= maxY;
    }
}