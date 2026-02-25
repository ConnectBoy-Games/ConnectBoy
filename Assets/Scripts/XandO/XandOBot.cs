using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class XandOBot
{
    private BotDifficulty difficulty;
    private string userPiece; //The player's piece
    private string botPiece; //The bot's piece

    public XandOBot(BotDifficulty difficulty, string userPiece, string botPiece)
    {
        this.difficulty = difficulty;
        this.userPiece = userPiece; 
        this.botPiece = botPiece;
    }

    public int ThinkMove(string[] gameState)
    {
        return difficulty switch
        {
            BotDifficulty.low => SimpleMoves(gameState),
            BotDifficulty.high => ComplexMoves(gameState),
            _ => Random.Range(0, 2) == 1 ? SimpleMoves(gameState) : ComplexMoves(gameState),
        };
    }

    public int SimpleMoves(string[] gameState)
    {
        Debug.Log("Thinking Simple Move");
        if (gameState[4] == "f") //f means the box is empty
        {
            return 4;
        }

        //Counterplays
        for (int i = 0; i < 3; i++)
        {
            //Rows Handling
            if (gameState[0 + i] == userPiece && gameState[6 + i] == userPiece && gameState[3 + i] == "f")
            {
                return 3 + i;
            }
            else if (gameState[0 + i] == userPiece && gameState[3 + i] == userPiece && gameState[6 + i] == "f")
            {
                return 6 + i;
            }
            else if (gameState[3 + i] == userPiece && gameState[6 + i] == userPiece && gameState[0 + i] == "f")
            {
                return 0 + i;
            }
            //Column Handling
            else if (gameState[0 + i] == userPiece && gameState[1 + i] == userPiece && gameState[2 + i] == "f")
            {
                return 2 + i;
            }
            else if (gameState[0 + i] == userPiece && gameState[2 + i] == userPiece && gameState[1 + i] == "f")
            {
                return 1 + i;
            }
            else if (gameState[1 + i] == userPiece && gameState[2 + i] == userPiece && gameState[0 + i] == "f")
            {
                return 0 + i;
            }
        }

        //R Diagonal Handling
        if (gameState[6] == userPiece && gameState[4] == userPiece && gameState[2] == "f")
        {
            return 2;
        }
        else if (gameState[6] == userPiece && gameState[2] == userPiece && gameState[4] == "f")
        {
            return 4;
        }
        else if (gameState[4] == userPiece && gameState[2] == userPiece && gameState[6] == "f")
        {
            return 6;
        }
        //L Diagonal Handling
        else if (gameState[0] == userPiece && gameState[4] == userPiece && gameState[8] == "f")
        {
            return 8;
        }
        else if (gameState[0] == userPiece && gameState[8] == userPiece && gameState[4] == "f")
        {
            return 4;
        }
        else if (gameState[4] == userPiece && gameState[8] == userPiece && gameState[0] == "f")
        {
            return 0;
        }

        //Else ... Play a random move
        for (int i = 0; i < 9; i++)
        {
            if (gameState[i] == "f")
            {
                return i;
            }
            //Else Game is a draw
        }

        return 0;
    }

    public int ComplexMoves(string[] gameState)
    {
        int bestScore = int.MinValue;
        int move = -1;

        // Loop through all spots on the board
        for (int i = 0; i < gameState.Length; i++)
        {
            if (gameState[i] == "f") // Check if the spot is available
            {
                gameState[i] = botPiece; // Temporarily make the move
                int score = Minimax(gameState, 0, false); // Call minimax
                gameState[i] = "f"; // Undo the move

                if (score > bestScore)
                {
                    bestScore = score;
                    move = i;
                }
            }
        }

        if(move == -1 || move > 9) return SimpleMoves(gameState);

        return move;
    }
    
    private int Minimax(string[] gameState, int depth, bool isMaximizing)
    {
        List<string> board = gameState.ToList<string>();

        string result = CheckWinner(board);
        if (result == botPiece) return 10 - depth; // Win for bot
        if (result == userPiece) return depth - 10; // Win for human
        if (result == "tie") return 0; // Draw

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < board.Count; i++)
            {
                if (board[i] == "f")
                {
                    board[i] = botPiece;
                    int score = Minimax(board.ToArray(), depth + 1, false);
                    board[i] = "f";
                    bestScore = System.Math.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < board.Count; i++)
            {
                if (board[i] == "f")
                {
                    board[i] = userPiece;
                    int score = Minimax(board.ToArray(), depth + 1, true);
                    board[i] = "f";
                    bestScore = System.Math.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    private string CheckWinner(List<string> b)
    {
        // All possible winning combinations
        int[,] winPatterns = new int[,] {
            {0,1,2}, {3,4,5}, {6,7,8}, // Rows
            {0,3,6}, {1,4,7}, {2,5,8}, // Columns
            {0,4,8}, {2,4,6}           // Diagonals
        };

        for (int i = 0; i < 8; i++)
        {
            if (b[winPatterns[i, 0]] != "f" &&
                b[winPatterns[i, 0]] == b[winPatterns[i, 1]] &&
                b[winPatterns[i, 0]] == b[winPatterns[i, 2]])
            {
                return b[winPatterns[i, 0]];
            }
        }

        if (b.All(s => s != "f")) return "tie";
        return null;
    }
}