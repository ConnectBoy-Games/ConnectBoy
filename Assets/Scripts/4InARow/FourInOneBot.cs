using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FourInOneBot
{
    private const int ROWS = 6;
    private const int COLS = 7;

    private BotDifficulty difficulty;
    private string userPiece;
    private string botPiece;

    public FourInOneBot(BotDifficulty difficulty, string userPiece, string botPiece)
    {
        this.difficulty = difficulty;
        this.userPiece = userPiece;
        this.botPiece = botPiece;
    }

    public int ThinkMove(int[,] gameState)
    {
        return difficulty switch
        {
            BotDifficulty.low => SimpleMoves(gameState),
            BotDifficulty.high => ComplexMoves(gameState),
            _ => Random.Range(0, 2) == 1 ? SimpleMoves(gameState) : ComplexMoves(gameState),
        };
    }

    public int SimpleMoves(int[,] gameState)
    {
        return GetNextMove(gameState, 3);
    }

    public int ComplexMoves(int[,] gameState)
    {
        return GetNextMove(gameState, 5);
    }

    public int GetNextMove(List<string> board, int depth)
    {
        // Difficulty: Depth 6 is "Expert", Depth 4 is "Challenging"
        int bestScore = int.MinValue;
        int bestCol = 3; // Start with center preference

        foreach (int col in GetValidColumns(board))
        {
            List<string> tempBoard = new List<string>(board);
            DropPiece(tempBoard, col, botPiece);

            int score = Minimax(tempBoard, depth, int.MinValue, int.MaxValue, false);

            if (score > bestScore)
            {
                bestScore = score;
                bestCol = col;
            }
        }
        return bestCol;
    }

    private int RunMinimax(List<string> board, int depth)
    {
        int bestScore = int.MinValue;
        // Prioritize center columns for better pruning performance
        int[] columnOrder = { 3, 2, 4, 1, 5, 0, 6 };
        int bestCol = 3;

        foreach (int col in columnOrder)
        {
            if (board[col] != "") continue; // Column is full

            List<string> tempBoard = new List<string>(board);
            DropPiece(tempBoard, col, botPiece);
            int score = Minimax(tempBoard, depth, int.MinValue, int.MaxValue, false);

            if (score > bestScore)
            {
                bestScore = score;
                bestCol = col;
            }
        }
        return bestCol;
    }

    private int Minimax(List<string> board, int depth, int alpha, int beta, bool maximizing)
    {
        bool isWin = CheckForWin(board, botPiece);
        bool isLoss = CheckForWin(board, userPiece);
        bool isFull = !board.Contains("");

        if (isWin) return 1000000 + depth;
        if (isLoss) return -1000000 - depth;
        if (isFull) return 0;
        if (depth == 0) return EvaluateBoard(board);

        if (maximizing)
        {
            int maxEval = int.MinValue;
            foreach (int col in GetValidColumns(board))
            {
                List<string> temp = new List<string>(board);
                DropPiece(temp, col, botPiece);
                int eval = Minimax(temp, depth - 1, alpha, beta, false);
                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (int col in GetValidColumns(board))
            {
                List<string> temp = new List<string>(board);
                DropPiece(temp, col, userPiece);
                int eval = Minimax(temp, depth - 1, alpha, beta, true);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }

    private int EvaluateBoard(List<string> board)
    {
        int totalScore = 0;

        // 1. Center Column Preference
        for (int r = 0; r < ROWS; r++)
            if (board[r * COLS + 3] == botPiece) totalScore += 5;

        // 2. Window-based scoring (Horizontal, Vertical, Diagonals)
        // Check for 3-in-a-row (open) = +10, 2-in-a-row = +2
        // If the human has 3-in-a-row (open) = -100 (high priority to block)

        return totalScore;
    }

    // Helper: Drops piece into the board list
    private void DropPiece(List<string> board, int col, string piece)
    {
        for (int r = ROWS - 1; r >= 0; r--)
        {
            if (board[r * COLS + col] == "")
            {
                board[r * COLS + col] = piece;
                break;
            }
        }
    }

    private List<int> GetValidColumns(List<string> board)
    {
        List<int> valid = new List<int>();
        // Check top row of each column
        for (int c = 0; c < COLS; c++)
            if (board[c] == "") valid.Add(c);
        return valid;
    }

    private bool CheckForWin(List<string> b, string p)
    {
        // Re-use the win-checking logic from the previous turn...
        // [Simplified for brevity - check Horizontal, Vertical, Diagonals]
        // Horizontal
        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS - 3; c++)
                if (b[r * COLS + c] == p && b[r * COLS + c + 1] == p && b[r * COLS + c + 2] == p && b[r * COLS + c + 3] == p) return true;
        // Vertical
        for (int r = 0; r < ROWS - 3; r++)
            for (int c = 0; c < COLS; c++)
                if (b[r * COLS + c] == p && b[(r + 1) * COLS + c] == p && b[(r + 2) * COLS + c] == p && b[(r + 3) * COLS + c] == p) return true;
        // Diagonal /
        for (int r = 3; r < ROWS; r++)
            for (int c = 0; c < COLS - 3; c++)
                if (b[r * COLS + c] == p && b[(r - 1) * COLS + c + 1] == p && b[(r - 2) * COLS + c + 2] == p && b[(r - 3) * COLS + c + 3] == p) return true;
        // Diagonal \
        for (int r = 0; r < ROWS - 3; r++)
            for (int c = 0; c < COLS - 3; c++)
                if (b[r * COLS + c] == p && b[(r + 1) * COLS + c + 1] == p && b[(r + 2) * COLS + c + 2] == p && b[(r + 3) * COLS + c + 3] == p) return true;

        return false;
    }
}
