using UnityEngine;

public class XandOBot
{
    private BotDifficulty difficulty;
    private string userPiece;
    private string botPiece;

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
        if (gameState[4] == "f")
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
            //else Game is a draw
        }

        return 0;
    }

    public int ComplexMoves(string[] gameState)
    {
        return SimpleMoves(gameState);
    }
}

