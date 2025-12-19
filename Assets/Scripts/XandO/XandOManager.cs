using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

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
    public Dictionary<int, int> gameState = new(); //Index, Type

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
                Invoke(nameof(MakeAIMove), 1f);
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            var temp = JsonConvert.SerializeObject(gameState, Formatting.Indented);
            Debug.Log(temp);
        }
    }

    public void MakeMove(int index)
    {
        if (gameState.TryGetValue(index, out var temp) == false && turnUser == User.host) //Valid move
        {
            if (gameMode == GameMode.vsPlayer)
            {
                //TODO: Send move to server
                SendMoveToServer();
            }
            else //Playing Against A Bot
            {
                gameState[index] = userPosition; //Update the game state
                PlacePiece(index, userPosition); //Place the piece on the board
                turnUser = User.bot;
                uiHandler.SetTurnText(turnUser);
                CheckBoardState(); //Check if there is a win
                Invoke(nameof(MakeAIMove), Random.Range(0.7f, 2.5f)); //Allow the bot make a move
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
                if (!gameState.TryGetValue(i, out _))
                {
                    return; //Game should keep on going (There's still free space)
                }
            }

            //Game is a draw
            uiHandler.SetTurnText(turnUser, "Draw!!!");
            Invoke(nameof(ClearBoard), 2f);
        }
    }

    /// <summary>Returns true if there is a win on the board</summary>
    private bool CheckWinState(int type)
    {
        int state1 = -1, state2 = -1, state3 = -1;

        if (gameState.TryGetValue(0, out state1) && gameState.TryGetValue(1, out state2) && gameState.TryGetValue(2, out state3)) //1
        {
            return (state1 == type) && (state2 == type) && (state3 == type);
        }
        state1 = -1; state2 = -1; state3 = -1;
        if (gameState.TryGetValue(3, out state1) && gameState.TryGetValue(4, out state2) && gameState.TryGetValue(5, out state3)) //2
        {
            return (state1 == type) && (state2 == type) && (state3 == type);
        }
        state1 = -1; state2 = -1; state3 = -1;
        if (gameState.TryGetValue(6, out state1) && gameState.TryGetValue(7, out state2) && gameState.TryGetValue(8, out state3)) //3
        {
            return (state1 == type) && (state2 == type) && (state3 == type);
        }
        state1 = -1; state2 = -1; state3 = -1;
        if (gameState.TryGetValue(0, out state1) && gameState.TryGetValue(3, out state2) && gameState.TryGetValue(6, out state3)) //4
        {
            return (state1 == type) && (state2 == type) && (state3 == type);
        }
        state1 = -1; state2 = -1; state3 = -1;
        if (gameState.TryGetValue(1, out state1) && gameState.TryGetValue(4, out state2) && gameState.TryGetValue(7, out state3)) //5
        {
            return (state1 == type) && (state2 == type) && (state3 == type);
        }
        state1 = -1; state2 = -1; state3 = -1;
        if (gameState.TryGetValue(2, out state1) && gameState.TryGetValue(5, out state2) && gameState.TryGetValue(8, out state3)) //6
        {
            return (state1 == type) && (state2 == type) && (state3 == type);
        }
        state1 = -1; state2 = -1; state3 = -1;
        if (gameState.TryGetValue(0, out state1) && gameState.TryGetValue(4, out state2) && gameState.TryGetValue(8, out state3)) //7
        {
            return (state1 == type) && (state2 == type) && (state3 == type);
        }
        state1 = -1; state2 = -1; state3 = -1;
        if (gameState.TryGetValue(6, out state1) && gameState.TryGetValue(4, out state2) && gameState.TryGetValue(2, out state3)) //8
        {
            return (state1 == type) && (state2 == type) && (state3 == type);
        }
        else
        {
            print("State 1: " + state1 + "State 2: " + state2 + "State 3: " + state3);
            return false;
        }
    }

    private void MakeAIMove()
    {
        int index = ThinkMove();
        gameState[index] = botPosition;
        PlacePiece(index, botPosition); //Place the piece on the board
        turnUser = User.host;
        uiHandler.SetTurnText(turnUser);
        CheckBoardState(); //Check if there is a win
    }

    private int ThinkMove()
    {
        if (!gameState.TryGetValue(4, out _))
        {
            return 4;
        }
        else //Do Counter Plays 
        {
            for (int i = 0; i < 3; i++)
            {
                //Rows Handling
                if (gameState.TryGetValue(0 + i, out _) && gameState.TryGetValue(6 + i, out _) && !gameState.TryGetValue(3 + i, out _))
                {
                    return 3 + i;
                }
                else if (gameState.TryGetValue(0 + i, out _) && gameState.TryGetValue(3 + i, out _) && !gameState.TryGetValue(6 + i, out _))
                {
                    return 6 + i;
                }
                else if (gameState.TryGetValue(3 + i, out _) && gameState.TryGetValue(6 + i, out _) && !gameState.TryGetValue(0 + i, out _))
                {
                    return 0 + i;
                }
                //Column Handling
                else if (gameState.TryGetValue(0 + i, out _) && gameState.TryGetValue(1 + i, out _) && !gameState.TryGetValue(2 + i, out _))
                {
                    return 2 + i;
                }
                else if (gameState.TryGetValue(0 + i, out _) && gameState.TryGetValue(2 + i, out _) && !gameState.TryGetValue(1 + i, out _))
                {
                    return 1 + i;
                }
                else if (gameState.TryGetValue(1 + i, out _) && gameState.TryGetValue(2 + i, out _) && !gameState.TryGetValue(0 + i, out _))
                {
                    return 0 + i;
                }
            }

            //R Diagonal Handling
            if (gameState.TryGetValue(6, out _) && gameState.TryGetValue(4, out _) && !gameState.TryGetValue(2, out _))
            {
                return 2;
            }
            else if (gameState.TryGetValue(6, out _) && gameState.TryGetValue(2, out _) && !gameState.TryGetValue(4, out _))
            {
                return 4;
            }
            else if (gameState.TryGetValue(4, out _) && gameState.TryGetValue(2, out _) && !gameState.TryGetValue(6, out _))
            {
                return 6;
            }
            //L Diagonal Handling
            else if (gameState.TryGetValue(0, out _) && gameState.TryGetValue(4, out _) && !gameState.TryGetValue(8, out _))
            {
                return 8;
            }
            else if (gameState.TryGetValue(0, out _) && gameState.TryGetValue(8, out _) && !gameState.TryGetValue(4, out _))
            {
                return 4;
            }
            else if (gameState.TryGetValue(4, out _) && gameState.TryGetValue(8, out _) && !gameState.TryGetValue(0, out _))
            {
                return 0;
            }

            //Else ... Play a random move
            for (int i = 0; i < 9; i++)
            {
                if (!gameState.TryGetValue(i, out _))
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
