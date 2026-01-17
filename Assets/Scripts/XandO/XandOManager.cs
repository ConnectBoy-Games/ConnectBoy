using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

public class XandOManager : MonoBehaviour
{
    //Internal Variables
    public static Wagr.Session gameSession;
    public static GameMode gameMode = GameMode.vsPlayer;

    private User turnUser; //Who has the turn?

    private string userPiece; //Either a 1 or 0 (X or O)
    private string botPiece; //Either a 1 or 0 (X or O)

    [SerializeField] XandOUIHandler uiHandler;

    [Header("UI Handling")]
    [SerializeField] private GameObject imageX; //0
    [SerializeField] private GameObject imageO; //1
    [SerializeField] private List<GameObject> buttons = new();

    private string[] gameState = new string[9];
    private XandOBot bot;

    SessionHandler sessionHandler;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        sessionHandler = new SessionHandler();

        ClearBoard();

        if (gameMode == GameMode.vsBot)
        {
            userPiece = (Random.Range(0, 2) == 1) ? "x" : "o"; //Set who has which piece
            botPiece = (userPiece == "x") ? "o" : "x"; //Set the alternative piece

            turnUser = (User)Random.Range(0, 2); //Set who has the turn
            uiHandler.SetTurnText(turnUser);

            bot = new XandOBot(BotDifficulty.medium, userPiece, botPiece);

            if (turnUser == User.bot)
            {
                Invoke(nameof(MakeAIMove), 1f);
            }
        }
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            var result = await sessionHandler.CreateSession();
        }
    }

    public void MakeMove(int index)
    {
        if (gameState[index] == "f" && turnUser == User.host) //Valid move
        {
            if (gameMode == GameMode.vsPlayer)
            {
                //TODO: Send move to server
                SendMoveToServer();
            }
            else //Playing Against A Bot
            {
                gameState[index] = userPiece; //Update the game state
                PlacePiece(index, userPiece); //Place the piece on the board
                turnUser = User.bot;
                uiHandler.SetTurnText(turnUser);
                CheckBoardState(); //Check if there is a win
                Invoke(nameof(MakeAIMove), Random.Range(0.7f, 2.5f)); //Allow the bot make a move
            }
        }
    }

    #region Board Update
    private void PlacePiece(int index, string type)
    {
        switch (type)
        {
            case "x": //X
                Instantiate(imageX, buttons[index].transform);
                break;
            case "o": //O
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
        gameState = new string[9];
        System.Array.Fill(gameState, "f");
    }
    #endregion

    #region AI Handling Functions
    private void CheckBoardState()
    {
        var win = CheckWinState(userPiece); //Check if player has won
        if (win != -1)
        {
            uiHandler.DisplayWinScreen("You Have Won");
        }

        win = CheckWinState(botPiece); //Check if bot has won
        if (win != -1)
        {
            uiHandler.DisplayWinScreen("Bot Has Won");
        }

        //No one has won yet
        for (int i = 0; i < 9; i++)
        {
            if (gameState[i] == "f") //There's an empty space
            {
                return; //Game should keep on going (There's still free space)
            }
        }

        //Game is a draw
        uiHandler.SetTurnText(turnUser, "Draw!!!");
        Invoke(nameof(ClearBoard), 2f);
    }

    /// <summary>Returns true if there is a win on the board</summary>
    private int CheckWinState(string playerId)
    {
        //Check Columns
        if (gameState[0] == playerId && gameState[1] == playerId && gameState[2] == playerId)
        {
            return 1;
        }
        else if (gameState[3] == playerId && gameState[4] == playerId && gameState[5] == playerId)
        {
            return 2;
        }
        else if (gameState[6] == playerId && gameState[7] == playerId && gameState[8] == playerId)
        {
            return 3;
        }
        //Check Rows
        else if (gameState[0] == playerId && gameState[3] == playerId && gameState[6] == playerId)
        {
            return 4;
        }
        else if (gameState[1] == playerId && gameState[4] == playerId && gameState[7] == playerId)
        {
            return 5;
        }
        else if (gameState[2] == playerId && gameState[5] == playerId && gameState[8] == playerId)
        {
            return 6;
        }
        //Check Diagonals
        else if (gameState[0] == playerId && gameState[4] == playerId && gameState[8] == playerId)
        {
            return 7;
        }
        else if (gameState[6] == playerId && gameState[4] == playerId && gameState[2] == playerId)
        {
            return 8;
        }
        else
        {
            return -1;
        }
    }

    private void MakeAIMove()
    {
        int index = bot.ThinkMove(gameState);
        gameState[index] = botPiece;
        PlacePiece(index, botPiece); //Place the piece on the board
        turnUser = User.host;
        uiHandler.SetTurnText(turnUser);
        CheckBoardState(); //Check if there is a win
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
