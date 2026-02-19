using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class FourInARowBot
{
    private BotDifficulty difficulty;
    private int userPiece;
    private int botPiece;
    private const int ROWS = 6;
    private const int COLS = 7;

    public FourInARowBot(BotDifficulty difficulty, int userPiece, int botPiece)
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

    public int GetNextMove(int[,] state, int depth)
    {
        // Difficulty: Depth 6 is "Expert", Depth 4 is "Challenging"
        int bestScore = int.MinValue;
        int bestCol = 3; // Start with center preference

        foreach (int col in GetValidColumns(state))
        {
            DropPiece(state, col, botPiece);

            int score = Minimax(state, depth, int.MinValue, int.MaxValue, false);

            if (score > bestScore)
            {
                bestScore = score;
                bestCol = col;
            }
        }
        return bestCol;
    }

    private int RunMinimax(int[,] state, int depth)
    {
        List<int> board = new();
        int bestScore = int.MinValue;
        // Prioritize center columns for better pruning performance
        int[] columnOrder = { 3, 2, 4, 1, 5, 0, 6 };
        int bestCol = 3;

        foreach (int col in columnOrder)
        {
            if (board[col] != 0) continue; // Column is full

            DropPiece(state, col, botPiece);
            int score = Minimax(state, depth, int.MinValue, int.MaxValue, false);

            if (score > bestScore)
            {
                bestScore = score;
                bestCol = col;
            }
        }
        return bestCol;
    }

    private int Minimax(int[,] state, int depth, int alpha, int beta, bool maximizing)
    {
        //bool isWin = CheckForWin(state, botPiece);
        //bool isLoss = CheckForWin(state, userPiece);
        //bool isFull = state. !board.Contains(0);

        //if (isWin) return 1000000 + depth;
        //if (isLoss) return -1000000 - depth;
        //if (isFull) return 0;
        if (depth == 0) return EvaluateBoard(state);

        if (maximizing)
        {
            int maxEval = int.MinValue;
            foreach (int col in GetValidColumns(state))
            {
                DropPiece(state, col, botPiece);
                int eval = Minimax(state, depth - 1, alpha, beta, false);
                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (int col in GetValidColumns(state))
            {
                DropPiece(state, col, userPiece);
                int eval = Minimax(state, depth - 1, alpha, beta, true);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }

    private int EvaluateBoard(int[,] state)
    {
        int totalScore = 0;

        // 1. Center Column Preference
        for (int r = 0; r < ROWS; r++)
            if (state[r, 3] == botPiece) totalScore += 5;

        // 2. Window-based scoring (Horizontal, Vertical, Diagonals)
        // Check for 3-in-a-row (open) = +10, 2-in-a-row = +2
        // If the human has 3-in-a-row (open) = -100 (high priority to block)

        return totalScore;
    }

    // Helper: Drops piece into the board list
    private void DropPiece(int[,] board, int col, int piece)
    {
        for (int r = ROWS - 1; r >= 0; r--)
        {
            if (board[r, col] == 0)
            {
                board[r, col] = piece;
                break;
            }
        }
    }

    private List<int> GetValidColumns(int[,] state)
    {
        List<int> valid = new List<int>();

        // Check top row of each column
        for (int c = 0; c < COLS; c++)
            if (state[0, c] == 0) valid.Add(c);

        return valid;
    }

    private bool CheckWin(int[,] state, int row, int col, User turnUser)
    {
        // The +1 is to count the piece just placed

        //Count Horizontally
        int hor = 1 + CountInDirection(state,row, col, 0, 1, turnUser) + CountInDirection(state, row, col, 0, -1, turnUser);

        //Vertically
        int ver = 1 + CountInDirection(state, row, col, 1, 0, turnUser) + CountInDirection(state, row, col, -1, 0, turnUser);

        //Diagonal (top-left to bottom-right),
        int diag1 = 1 + CountInDirection(state, row, col, 1, 1, turnUser) + CountInDirection(state, row, col, -1, -1, turnUser);

        //Diagonal (top-right to bottom-left)
        int diag2 = 1 + CountInDirection(state, row, col, 1, -1, turnUser) + CountInDirection(state,    row, col, -1, 1, turnUser);

        if (hor >= 4 || ver >= 4 || diag1 >= 4 || diag2 >= 4)
        {
            return true;
        }
        return false;
    }

    /// <summary>Counts the number of the same pieces in a direction and returns the count!</summary>
    private int CountInDirection(int[,] state, int row, int col, int dr, int dc, User user)
    {
        //(row,col) is the starting point, (dr, dc) is the direction to move in,
        int count = 0;
        int r = row + dr;
        int c = col + dc;

        // Move in the direction as long as we find the same player's piece
        while (r >= 0 && r < ROWS && c >= 0 && c < COLS && state[r, c] == (int)user)
        {
            count++;
            r += dr;
            c += dc;
        }
        return count;
    }

    private bool CheckForWin(List<int> b, int p)
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
