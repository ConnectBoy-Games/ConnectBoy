using System.Collections.Generic;
using UnityEngine;

public class XandOManager : MonoBehaviour
{
    private XandOBot bot; //X And O bot
    private User turnUser; //Who has the turn?

    private string userPiece; // (x or o)
    private string botPiece; // (x or o)

    private string[] gameState = new string[9];

    [SerializeField] XandOUIHandler uiHandler;

    [Header("UI Handling")]
    [SerializeField] private GameObject imageX; //0
    [SerializeField] private GameObject imageO; //1
    [SerializeField] private List<GameObject> buttons = new();
    [SerializeField] private List<GameObject> winLines = new();

    public void Start()
    {
        ClearBoard();

        if (GameManager.gameSession.gameMode == GameMode.vsBot)
        {
            userPiece = (Random.Range(0, 2) == 1) ? "x" : "o"; //Set who has which piece
            botPiece = (userPiece == "x") ? "o" : "x"; //Set the alternative piece
            turnUser = (User)Random.Range(0, 2); //Set who has the turn

            uiHandler.SetTurnText(turnUser);
            bot = new XandOBot(BotDifficulty.medium, userPiece);

            if (turnUser == User.bot)
            {
                Invoke(nameof(MakeAIMove), 1f);
            }
        }
    }

    //For allowing the player to make a move
    public void MakeMove(int index)
    {
        if (gameState[index] == "f" && turnUser == User.host) //Valid move
        {
            if (GameManager.gameSession.gameMode == GameMode.vsBot)
            {
                gameState[index] = userPiece; //Update the game state
                PlacePiece(index, userPiece); //Place the piece on the board
                turnUser = User.bot; //Hand over the turn
                uiHandler.SetTurnText(turnUser); //Display the turn text
                CheckBoardState(); //Check if there is a win

                Invoke(nameof(MakeAIMove), Random.Range(0.7f, 2.5f)); //Allow the bot make a move
            }
            else //Playing Against A Bot
            {
                //TODO: Send move to server
                //SendMoveToServer();
            }
        }
    }

    private void MakeAIMove()
    {
        int index = bot.ThinkMove(gameState); //Let the bot think a move
        gameState[index] = botPiece; //Update the game state
        PlacePiece(index, botPiece); //Place the bot piece on the board
        turnUser = User.host; //Hand over the turn
        uiHandler.SetTurnText(turnUser); //Display the turn text
        CheckBoardState(); //Check if there is a win
    }

    #region Board UI Update
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

    private void ActivateWinLine(int index)
    {
        winLines[index].gameObject.SetActive(true);
    }

    private void ClearBoard()
    {
        foreach (GameObject button in buttons)
        {
            foreach (Transform piece in button.transform)
            {
                Destroy(piece.gameObject);
            }
        }

        gameState = new string[9];
        System.Array.Fill(gameState, "f");
    }
    #endregion

    #region Board Check
    private void CheckBoardState()
    {
        var win = CheckWinState(userPiece); //Check if player has won

        if (win != -1)
        {
            ActivateWinLine(win);
            uiHandler.DisplayWinScreen("You Have Won");
        }

        win = CheckWinState(botPiece); //Check if bot has won
        if (win != -1)
        {
            ActivateWinLine(win);
            uiHandler.DisplayWinScreen("Bot Has Won");
        }

        //No one has won yet
        for (int i = 0; i < 9; i++)
        {
            if (gameState[i] == "f") //There's an empty space
                return; //Game should keep on going (There's still free space)
        }

        //Game is a draw
        uiHandler.SetTurnText(turnUser, "Draw!!!");
        Invoke(nameof(ClearBoard), 1f);
    }

    /// <summary>Returns the direction of the win or -1 if there is no win</summary>
    private int CheckWinState(string piece)
    {
        //Check Columns
        if (gameState[0] == piece && gameState[1] == piece && gameState[2] == piece)
        {
            return 1;
        }
        else if (gameState[3] == piece && gameState[4] == piece && gameState[5] == piece)
        {
            return 2;
        }
        else if (gameState[6] == piece && gameState[7] == piece && gameState[8] == piece)
        {
            return 3;
        }
        //Check Rows
        else if (gameState[0] == piece && gameState[3] == piece && gameState[6] == piece)
        {
            return 4;
        }
        else if (gameState[1] == piece && gameState[4] == piece && gameState[7] == piece)
        {
            return 5;
        }
        else if (gameState[2] == piece && gameState[5] == piece && gameState[8] == piece)
        {
            return 6;
        }
        //Check Diagonals
        else if (gameState[0] == piece && gameState[4] == piece && gameState[8] == piece)
        {
            return 7;
        }
        else if (gameState[6] == piece && gameState[4] == piece && gameState[2] == piece)
        {
            return 8;
        }
        else
        {
            return -1;
        }
    }
    #endregion
}
