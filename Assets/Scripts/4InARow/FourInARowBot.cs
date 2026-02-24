using System.Collections.Generic;
using UnityEngine;

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
            BotDifficulty.medium => ComplexMoves(gameState, 4),
            BotDifficulty.high => ComplexMoves(gameState, 6),
            _ => SimpleMoves(gameState),
        };
    }

    public int SimpleMoves(int[,] gameState)
    {
        // Low difficulty uses a shallow search depth
        return GetNextMove(gameState, 3);
    }

    public int ComplexMoves(int[,] gameState, int depth = 5)
    {
        // High difficulty uses a deeper search depth
        return GetNextMove(gameState, depth);
    }

    public int GetNextMove(int[,] state, int depth)
    {
        int bestScore = int.MinValue;
        int bestCol = 3; // Center preference

        // Clone the state to avoid modifying the actual game board during simulation
        int[,] stateClone = (int[,])state.Clone();

        // Heuristic: prioritize center columns for better pruning performance
        int[] columnOrder = { 3, 2, 4, 1, 5, 0, 6 };

        foreach (int col in columnOrder)
        {
            if (!IsValidColumn(stateClone, col)) continue;

            DropPiece(stateClone, col, botPiece);
            // After our move, it's the opponent's turn (minimizing)
            int score = Minimax(stateClone, depth - 1, int.MinValue, int.MaxValue, false);
            RemovePiece(stateClone, col);

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
        bool isWin = CheckForWin(state, botPiece);
        bool isLoss = CheckForWin(state, userPiece);
        bool isFull = IsBoardFull(state);

        if (isWin) return 1000000 + depth;
        if (isLoss) return -1000000 - depth;
        if (isFull) return 0;
        if (depth == 0) return EvaluateBoard(state);

        int[] columnOrder = { 3, 2, 4, 1, 5, 0, 6 };

        if (maximizing)
        {
            int maxEval = int.MinValue;
            foreach (int col in columnOrder)
            {
                if (!IsValidColumn(state, col)) continue;

                DropPiece(state, col, botPiece);
                int eval = Minimax(state, depth - 1, alpha, beta, false);
                RemovePiece(state, col);

                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (int col in columnOrder)
            {
                if (!IsValidColumn(state, col)) continue;

                DropPiece(state, col, userPiece);
                int eval = Minimax(state, depth - 1, alpha, beta, true);
                RemovePiece(state, col);

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

        // 2. Window-based scoring
        // Horizontal
        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS - 3; c++)
                totalScore += ScoreWindow(state[r, c], state[r, c + 1], state[r, c + 2], state[r, c + 3]);

        // Vertical
        for (int c = 0; c < COLS; c++)
            for (int r = 0; r < ROWS - 3; r++)
                totalScore += ScoreWindow(state[r, c], state[r + 1, c], state[r + 2, c], state[r + 3, c]);

        // Diagonal /
        for (int r = 3; r < ROWS; r++)
            for (int c = 0; c < COLS - 3; c++)
                totalScore += ScoreWindow(state[r, c], state[r - 1, c + 1], state[r - 2, c + 2], state[r - 3, c + 3]);

        // Diagonal \
        for (int r = 0; r < ROWS - 3; r++)
            for (int c = 0; c < COLS - 3; c++)
                totalScore += ScoreWindow(state[r, c], state[r + 1, c + 1], state[r + 2, c + 2], state[r + 3, c + 3]);

        return totalScore;
    }

    private int ScoreWindow(int p1, int p2, int p3, int p4)
    {
        int score = 0;
        int botCount = 0;
        int userCount = 0;
        int emptyCount = 0;

        int[] window = { p1, p2, p3, p4 };
        foreach (int p in window)
        {
            if (p == botPiece) botCount++;
            else if (p == userPiece) userCount++;
            else if (p == 0) emptyCount++;
        }

        // Scoring for bot
        if (botCount == 4) score += 10000;
        else if (botCount == 3 && emptyCount == 1) score += 100;
        else if (botCount == 2 && emptyCount == 2) score += 10;

        // Scoring for user (penalties to block)
        if (userCount == 3 && emptyCount == 1) score -= 500;
        else if (userCount == 2 && emptyCount == 2) score -= 50;

        return score;
    }

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

    private void RemovePiece(int[,] board, int col)
    {
        for (int r = 0; r < ROWS; r++)
        {
            if (board[r, col] != 0)
            {
                board[r, col] = 0;
                break;
            }
        }
    }

    private bool IsValidColumn(int[,] state, int col)
    {
        return state[0, col] == 0;
    }

    private bool IsBoardFull(int[,] state)
    {
        for (int c = 0; c < COLS; c++)
            if (state[0, c] == 0) return false;
        return true;
    }

    private bool CheckForWin(int[,] b, int p)
    {
        // Horizontal
        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS - 3; c++)
                if (b[r, c] == p && b[r, c + 1] == p && b[r, c + 2] == p && b[r, c + 3] == p) return true;
        // Vertical
        for (int r = 0; r < ROWS - 3; r++)
            for (int c = 0; c < COLS; c++)
                if (b[r, c] == p && b[r + 1, c] == p && b[r + 2, c] == p && b[r + 3, c] == p) return true;
        // Diagonal /
        for (int r = 3; r < ROWS; r++)
            for (int c = 0; c < COLS - 3; c++)
                if (b[r, c] == p && b[r - 1, c + 1] == p && b[r - 2, c + 2] == p && b[r - 3, c + 3] == p) return true;
        // Diagonal \
        for (int r = 0; r < ROWS - 3; r++)
            for (int c = 0; c < COLS - 3; c++)
                if (b[r, c] == p && b[r + 1, c + 1] == p && b[r + 2, c + 2] == p && b[r + 3, c + 3] == p) return true;

        return false;
    }
}

