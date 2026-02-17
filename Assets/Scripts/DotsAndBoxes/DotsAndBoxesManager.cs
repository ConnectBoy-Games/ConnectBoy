using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotsAndBoxesManager : MonoBehaviour, IGameManager
{
    private DotsAndBoxesState localState;
    private DotsAndBoxesBot bot;
    private User turnUser;

    [Header("UI Handling")]
    [SerializeField] DotsAndBoxesUIHandler uiHandler;

    [Header("GameBoard Handling")]
    [SerializeField] Image[] tiles;
    [SerializeField] Image[] buttons;

    public int playerPoint{get; private set;}
    public int botPoint{get; private set;}

    [SerializeField] List<int> gameState;

    void Start()
    {
        if (GameManager.gameMode == GameMode.vsBot)
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

    public void SwitchTurns()
    {
        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (turnUser == User.bot) ? User.client : User.bot;
                break;
            case GameMode.vsPlayer:
                turnUser = (turnUser == User.client) ? User.player : User.client;
                break;
            case GameMode.online:
                turnUser = (turnUser == User.client) ? User.player : User.client;
                break;
        }
        uiHandler.SetTurnText(turnUser); //Display the turn text
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

    public void ClearBoard()
    {
        throw new System.NotImplementedException();
    }

    public int CheckWinState(string piece)
    {
        throw new System.NotImplementedException();
    }

    public void GetGameState()
    {
        throw new System.NotImplementedException();
    }
}
