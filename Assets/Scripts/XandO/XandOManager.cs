using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class XandOManager : MonoBehaviour
{
    //Internal Variables
    public static Wagr.Session gameSession;
    public static GameMode gameMode;
    private User turnUser; //Who has the turn?
    private int userPosition; //Either a 1 or 0 (X or O)
    private int botPosition; //Either a 1 or 0 (X or O)

    [SerializeField] XandOUIHandler uiHandler;

    [Header("UI Handling")]
    [SerializeField] private GameObject imageX; //0
    [SerializeField] private GameObject imageO; //1
    [SerializeField] private List<GameObject> buttons = new();
    [SerializeField] private Dictionary<int, int> gameState = new(); //Index, Type

    void Start()
    {
        ClearBoard();

        if (gameMode == GameMode.vsBot)
        {
            userPosition = Random.Range(0, 2); //Set who has which piece
            botPosition = (userPosition == 1) ? 0 : 1; //Set the alternative piece

            turnUser = (User)Random.Range(0, 2); //Set who has the turn
            uiHandler.SetTurnText(turnUser);

            if (turnUser == User.bot)
            {
                Invoke(nameof(MakeAIMove), 0.3f);
            }
        }
    }

    public void MakeMove(int index)
    {
        if (!gameState.ContainsKey(index) && turnUser == User.host) //Valid move
        {
            if (gameMode == GameMode.vsPlayer)
            {
                //TODO: Send move to server
                SendMoveToServer();
            }
            else //BotMode
            {
                gameState[index] = userPosition; //Update the game state
                PlacePiece(index, userPosition); //Place the piece on the board
                CheckBoardState(); //Check if there is a win
                turnUser = User.bot;
                uiHandler.SetTurnText(turnUser);
                Invoke(nameof(MakeAIMove), 0.3f); //Allow the bot make a move
            }
        }
    }

    #region Board Update
    private void PlacePiece(int index, int type)
    {
        switch (type)
        {
            case 0: //X
                Instantiate(imageX, buttons[index].transform);
                break;
            case 1: //O
                Instantiate(imageO, buttons[index].transform);
                break;
        }
    }

    private void ClearBoard()
    {
        foreach (GameObject button in buttons)
        {
            for (int i = 0; i < button.transform.childCount; i++)
            {
                Destroy(button.transform.GetChild(i).gameObject);
            }
        }
        gameState = new();
    }

    
    #endregion

    #region AI Handling Functions
    private void CheckBoardState()
    {
        if (CheckWinState(0)) //X has won
        {
            if (userPosition == 0) //User has won
            {
                uiHandler.DisplayWinScreen("You Have Won");
            }
            else //Bot has won
            {
                uiHandler.DisplayWinScreen("Bot Has Won");
            }
        }
        else if (CheckWinState(1)) //O has won
        {
            if (userPosition == 1) //User has won
            {
                uiHandler.DisplayWinScreen("You Have Won");
            }
            else //Bot has won
            {
                uiHandler.DisplayWinScreen("Bot Has Won");
            }
        }
        else
        {
            //No one has won yet
            for (int i = 0; i < 9; i++)
            {
                if (!gameState.ContainsKey(i))
                {
                    break; //Game should keep on going
                }
                else
                {
                    //Game is a draw

                    Invoke(nameof(ClearBoard), 0.5f);
                }
            }
        }
    }

    /// <summary>Returns true if there is a win on the board</summary>
    private bool CheckWinState(int type)
    {
        if (gameState[0] == type && gameState[1] == type && gameState[2] == type) //1
        {
            return true;
        }
        else if (gameState[3] == type && gameState[4] == type && gameState[5] == type) //2
        {
            return true;
        }
        else if (gameState[6] == type && gameState[7] == type && gameState[8] == type) //3
        {
            return true;
        }
        else if (gameState[0] == type && gameState[3] == type && gameState[6] == type) //4
        {
            return true;
        }
        else if (gameState[1] == type && gameState[4] == type && gameState[7] == type) //5
        {
            return true;
        }
        else if (gameState[2] == type && gameState[5] == type && gameState[8] == type) //6
        {
            return true;
        }
        else if (gameState[0] == type && gameState[4] == type && gameState[8] == type) //7
        {
            return true;
        }
        else if (gameState[6] == type && gameState[4] == type && gameState[2] == type) //8
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MakeAIMove()
    {
        int index = ThinkMove();
        gameState[index] = botPosition;
        PlacePiece(index, botPosition); //Place the piece on the board
        CheckBoardState(); //Check if there is a win
        turnUser = User.host;
        uiHandler.SetTurnText(turnUser);
    }

    private int ThinkMove()
    {
        if (!gameState.ContainsKey(4))
        {
            return 4;
        }
        else //Do Counter Plays 
        {
            for (int i = 0; i < 3; i++)
            {
                //Rows Handling
                if (gameState.ContainsKey(0 + i) && gameState.ContainsKey(6 + i))
                {
                    return 3 + i;
                }
                else if (gameState.ContainsKey(0 + i) && gameState.ContainsKey(3 + i))
                {
                    return 6 + i;
                }
                else if (gameState.ContainsKey(3 + i) && gameState.ContainsKey(6 + i))
                {
                    return 0 + i;
                }
                //Column Handling
                else if (gameState.ContainsKey(0 + i) && gameState.ContainsKey(1 + i))
                {
                    return 2 + i;
                }
                else if (gameState.ContainsKey(0 + i) && gameState.ContainsKey(2 + i))
                {
                    return 1 + i;
                }
                else if (gameState.ContainsKey(1 + i) && gameState.ContainsKey(2 + i))
                {
                    return 0 + i;
                }
            }

            //R Diagonal Handling
            if (gameState.ContainsKey(6) && gameState.ContainsKey(4))
            {
                return 2;
            }
            else if (gameState.ContainsKey(6) && gameState.ContainsKey(2))
            {
                return 4;
            }
            else if (gameState.ContainsKey(4) && gameState.ContainsKey(2))
            {
                return 6;
            }
            //L Diagonal Handling
            else if (gameState.ContainsKey(0) && gameState.ContainsKey(4))
            {
                return 8;
            }
            else if (gameState.ContainsKey(0) && gameState.ContainsKey(8))
            {
                return 4;
            }
            else if (gameState.ContainsKey(4) && gameState.ContainsKey(8))
            {
                return 0;
            }

            //Else ... Play a random move
            for (int i = 0; i < 9; i++)
            {
                if (!gameState.ContainsKey(i))
                {
                    return i;
                }
                else
                {
                    return 0;
                    //Game is a draw
                }
            }

            return 0;
        }
    }
    #endregion

    #region Server Connecting Functions
    public void ConnectToServer()
    {

    }

    public void SendMoveToServer()
    {

    }

    public void SendChatToServer()
    {

    }
    #endregion
}
