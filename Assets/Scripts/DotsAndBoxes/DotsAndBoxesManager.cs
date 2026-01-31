using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotsAndBoxesManager : MonoBehaviour
{
    [SerializeField] DotsAndBoxesUIHandler uiHandler;
    [SerializeField] Image[] tiles;
    [SerializeField] Image[] buttons;
    private DotsAndBoxesBot bot;

    public int playerPoint{get; private set;}
    public int botPoint{get; private set;}
    private User turnUser;
    private GameMode gameMode;

    [SerializeField] List<int> gameState;

    void Start()
    {
        if (gameMode == GameMode.vsBot)
        {
            turnUser = (User)Random.Range(0, 2); //Set who has the turn
            uiHandler.SetTurnText(turnUser);

            bot = new DotsAndBoxesBot(BotDifficulty.medium);

            if (turnUser == User.bot)
            {
                Invoke(nameof(MakeAIMove), 1f);
            }
        }
        playerPoint = 0;
        botPoint = 0;
        gameState = new();

        foreach(var tile in tiles)
        {
            tile.CrossFadeAlpha(0, 0.1f, true); //Fadeout all the tiles
        }
    }

    public void PlayMove(int move)
    {
        if(gameState.Contains(move))
        {
            return;
        }
        else
        {
            //Play move
            gameState.Add(move);
            SetButtonColor(move);

            var check = CheckMove(move);
            if(check != -1)
            {
                SetTileColor(check, turnUser);
            }
        }
    }

    public void MakeAIMove()
    {
        int move = bot.ThinkMove(gameState);
        gameState.Add(move);
        SetButtonColor(move);
        var check = CheckMove(move);
        if(check != -1)
        {
            SetTileColor(check, turnUser);
            turnUser = User.bot;
        }
        else
        {
            turnUser = User.client;
        }
        uiHandler.SetTurnText(turnUser);
        CheckBoardState(); //Check if there is a win
    }

    public int CheckMove(int move)
    {

        return -1;
    }

    public void CheckBoardState()
    {
        
    }

    public void SetButtonColor(int button)
    {
        buttons[button - 1].CrossFadeColor(Color.red, 1f, true, false);
    }

    public void SetTileColor(int tile, User user)
    {
        tiles[tile - 1].CrossFadeAlpha(1, 1, true);
        tiles[tile - 1].CrossFadeColor(Color.green, 1f, true, false);
    }
}
