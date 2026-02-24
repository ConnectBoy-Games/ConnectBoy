using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DotsAndBoxesBot
{
    public BotDifficulty difficulty;
    private const int Rows = 5;
    private const int Cols = 5;
    private const int TotalHorizontal = 30; // (Rows + 1) * Cols
    private const int TotalVertical = 30;   // Rows * (Cols + 1)

    public DotsAndBoxesBot(BotDifficulty difficulty)
    {
        this.difficulty = difficulty;
    }

    /// <summary> Thinks and returns the best move for the current state.</summary>
    /// <param name="horizontalEdges">List of Indices of horizontal edges already played.</param>
    /// <param name="verticalEdges">List of Indices of vertical edges already played.</param>
    /// <param name="boxes">List of box indices already claimed.</param>
    /// <returns>A DaBMove object containing the move details.</returns>
    public DaBMove ThinkMove(List<int> horizontalEdges, List<int> verticalEdges)
    {
        return difficulty switch
        {
            BotDifficulty.low => GetEasyMove(horizontalEdges, verticalEdges),
            BotDifficulty.high => GetHardMove(horizontalEdges, verticalEdges),
            _ => Random.Range(0, 2) == 1 ? GetEasyMove(horizontalEdges, verticalEdges) : GetHardMove(horizontalEdges, verticalEdges),
        };
    }

    private DaBMove GetEasyMove(List<int> horizontalEdges, List<int> verticalEdges)
    {
        var availableMoves = GetAvailableMoves(horizontalEdges, verticalEdges);
        if (availableMoves.Count == 0) return null;

        foreach (var move in availableMoves)
        {
            if (CompletesABox(move, horizontalEdges, verticalEdges))
            {
                return move;
            }
        }

        return availableMoves[Random.Range(0, availableMoves.Count)];
    }

    private DaBMove GetHardMove(List<int> horizontalEdges, List<int> verticalEdges)
    {
        var availableMoves = GetAvailableMoves(horizontalEdges, verticalEdges);
        if (availableMoves.Count == 0) return null;

        //Take any move that completes a box
        foreach (var move in availableMoves)
        {
            if (CompletesABox(move, horizontalEdges, verticalEdges))
            {
                return move;
            }
        }

        //Avoid moves that create a third side (giving the opponent a box)
        var safeMoves = availableMoves.Where(move => !CreatesThirdSide(move, horizontalEdges, verticalEdges)).ToList();
        if (safeMoves.Count > 0)
        {
            return safeMoves[Random.Range(0, safeMoves.Count)];
        }

        //If no safe moves, just pick a random move (could be improved to find smallest chain)
        return availableMoves[Random.Range(0, availableMoves.Count)];
    }

    private List<DaBMove> GetAvailableMoves(List<int> horizontalEdges, List<int> verticalEdges)
    {
        List<DaBMove> moves = new List<DaBMove>();

        for (int i = 0; i < TotalHorizontal; i++)
        {
            if (!horizontalEdges.Contains(i))
                moves.Add(new DaBMove { H = i, V = -1 });
        }

        for (int i = 0; i < TotalVertical; i++)
        {
            if (!verticalEdges.Contains(i))
                moves.Add(new DaBMove { H = -1, V = i });
        }

        return moves;
    }

    private bool CompletesABox(DaBMove move, List<int> horizontalEdges, List<int> verticalEdges)
    {
        if (move.H != -1) //Check if it's a horizontal edge
        {
            int r = move.H / Cols;
            int c = move.H % Cols;

            // Edge is the top of box (r, c)
            if (r < Rows && SidesInBox(r, c, horizontalEdges, verticalEdges, move) == 4) return true;
            // Edge is the bottom of box (r-1, c)
            if (r > 0 && SidesInBox(r - 1, c, horizontalEdges, verticalEdges, move) == 4) return true;
        }
        else // Check if it's a vertical edge
        {
            int r = move.V / (Cols + 1);
            int c = move.V % (Cols + 1);

            // Edge is the left of box (r, c)
            if (c < Cols && SidesInBox(r, c, horizontalEdges, verticalEdges, move) == 4) return true;
            // Edge is the right of box (r, c-1)
            if (c > 0 && SidesInBox(r, c - 1, horizontalEdges, verticalEdges, move) == 4) return true;
        }

        return false;
    }

    private bool CreatesThirdSide(DaBMove move, List<int> horizontalEdges, List<int> verticalEdges)
    {
        if (move.H != -1)
        {
            int r = move.H / Cols;
            int c = move.H % Cols;

            if (r < Rows && SidesInBox(r, c, horizontalEdges, verticalEdges, move) == 3) return true;
            if (r > 0 && SidesInBox(r - 1, c, horizontalEdges, verticalEdges, move) == 3) return true;
        }
        else
        {
            int r = move.V / (Cols + 1);
            int c = move.V % (Cols + 1);

            if (c < Cols && SidesInBox(r, c, horizontalEdges, verticalEdges, move) == 3) return true;
            if (c > 0 && SidesInBox(r, c - 1, horizontalEdges, verticalEdges, move) == 3) return true;
        }

        return false;
    }

    private int SidesInBox(int r, int c, List<int> hEdges, List<int> vEdges, DaBMove pendingMove)
    {
        int sides = 0;

        int topH = r * Cols + c;
        int bottomH = (r + 1) * Cols + c;
        int leftV = r * (Cols + 1) + c;
        int rightV = r * (Cols + 1) + c + 1;

        if (hEdges.Contains(topH) || pendingMove.H == topH) sides++;
        if (hEdges.Contains(bottomH) || pendingMove.H == bottomH) sides++;
        if (vEdges.Contains(leftV) || pendingMove.V == leftV) sides++;
        if (vEdges.Contains(rightV) || pendingMove.V == rightV) sides++;

        return sides;
    }
}
