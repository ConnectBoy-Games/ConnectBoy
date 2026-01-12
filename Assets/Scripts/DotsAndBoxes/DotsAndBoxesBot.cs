using System.Collections.Generic;
using UnityEngine;

public class DotsAndBoxesBot : MonoBehaviour
{
    public BotDifficulty difficulty;

    public DotsAndBoxesBot(BotDifficulty difficulty)
    {
        this.difficulty = difficulty;
    }

    public int ThinkMove(List<int> gameState)
    {
        for(int i = 1; i < 61; i++)
        {
            if(!gameState.Contains(i))
            {
                return i;
            }
        }

        return 0; //A filled board
    }
}
