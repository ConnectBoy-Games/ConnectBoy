using UnityEngine;

public class FourInOneBot
{
    private BotDifficulty difficulty;
    private string userPiece;
    private string botPiece;

    public FourInOneBot(BotDifficulty difficulty, string userPiece, string botPiece)
    {
        this.difficulty = difficulty;
        this.userPiece = userPiece;
        this.botPiece = botPiece;
    }

    public int ThinkMove(int[][] gameState)
    {
        return difficulty switch
        {
            BotDifficulty.low => SimpleMoves(gameState),
            BotDifficulty.high => ComplexMoves(gameState),
            _ => Random.Range(0, 2) == 1 ? SimpleMoves(gameState) : ComplexMoves(gameState),
        };
    }

    public int SimpleMoves(int[][] gameState)
    {
        return 0;
    }

    public int ComplexMoves(int[][] gameState)
    {
        return SimpleMoves(gameState);
    }
}
