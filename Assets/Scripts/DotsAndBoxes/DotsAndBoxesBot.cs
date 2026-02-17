using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DotsAndBoxesBot
{
    public BotDifficulty difficulty;
    private int gridWidth = 4; // 5x5 dots = 4x4 boxes
    private int totalEdges = 40;

    public DotsAndBoxesBot(BotDifficulty difficulty)
    {
        this.difficulty = difficulty;
    }

    public int ThinkMove(List<string> gameState)
    {
        return difficulty switch
        {
            BotDifficulty.low => GetRandomMove(gameState),
            BotDifficulty.medium => GetMediumMove(gameState),
            BotDifficulty.high => GetHardMove(gameState),
            _ => GetRandomMove(gameState),
        };
    }

    // Just pick any available edge
    private int GetRandomMove(List<string> board)
    {
        List<int> available = GetAvailableMoves(board);
        return available[Random.Range(0, available.Count)];
    }

    // Take boxes if possible, otherwise avoid giving them
    private int GetMediumMove(List<string> board)
    {
        List<int> available = GetAvailableMoves(board);

        // 1. Try to finish a box
        foreach (int move in available)
            if (CompletesBox(board, move)) return move;

        // 2. Try to avoid giving the opponent a box (don't be the 3rd side)
        List<int> safeMoves = available.Where(m => !CreatesThirdSide(board, m)).ToList();
        if (safeMoves.Count > 0) return safeMoves[Random.Range(0, safeMoves.Count)];

        return GetRandomMove(board);
    }

    // Alpha-Beta Pruning
    private int GetHardMove(List<string> board)
    {
        int bestMove = -1;
        int bestValue = int.MinValue;
        int depth = 4; // Adjust for performance

        foreach (int move in GetAvailableMoves(board))
        {
            board[move] = "O";
            int moveValue = Minimax(board, depth, int.MinValue, int.MaxValue, false);
            board[move] = "f";

            if (moveValue > bestValue)
            {
                bestValue = moveValue;
                bestMove = move;
            }
        }
        return bestMove == -1 ? GetRandomMove(board) : bestMove;
    }

    private int Minimax(List<string> board, int depth, int alpha, int beta, bool isMaximizing)
    {
        if (depth == 0 || IsBoardFull(board)) return EvaluateBoard(board);

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            foreach (int move in GetAvailableMoves(board))
            {
                board[move] = "O";
                int eval = Minimax(board, depth - 1, alpha, beta, false);
                board[move] = "f";
                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (int move in GetAvailableMoves(board))
            {
                board[move] = "X";
                int eval = Minimax(board, depth - 1, alpha, beta, true);
                board[move] = "f";
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }

    private bool CompletesBox(List<string> board, int move)
    {
        // Logic to check if placing this edge closes a 1x1 square
        // This requires mapping edge indices to box structures
        return false; // Implement based on your specific index mapping
    }

    private bool CreatesThirdSide(List<string> board, int move)
    {
        // Check if placing this edge makes any box have 3 sides filled
        return false; // Implement based on your specific index mapping
    }

    private int EvaluateBoard(List<string> board)
    {
        // Heuristic: Count completed boxes for Bot minus boxes for Player
        return 0;
    }

    private List<int> GetAvailableMoves(List<string> board)
    {
        List<int> moves = new List<int>();
        for (int i = 0; i < board.Count; i++)
            if (board[i] == "f") moves.Add(i);
        return moves;
    }

    private bool IsBoardFull(List<string> board) => !board.Contains("f");
}
