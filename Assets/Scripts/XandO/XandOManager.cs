using System.Collections.Generic;
using UnityEngine;

public class XandOManager : MonoBehaviour
{
    private XandOBot bot; //X And O bot
    private User turnUser; //Who has the turn?

    private string userPiece; // (x or o)
    private string botPiece; // (x or o)
    private bool isGameOver = false;

    private XAndOState state;

    [SerializeField] XandOUIHandler uiHandler;

    [Header("UI Handling")]
    [SerializeField] private GameObject imageX; //0
    [SerializeField] private GameObject imageO; //1
    [SerializeField] private List<GameObject> buttons = new();
    [SerializeField] private List<GameObject> winLines = new();

    public void Start()
    {
        state.Board = new string[9];
        System.Array.Fill(state.Board, "f");

        isGameOver = false;
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
    public async void MakeMove(int index)
    {
        if (state.Board[index] == "f" && turnUser == User.host && !isGameOver) //Valid move
        {
            if (GameManager.gameSession.gameMode == GameMode.vsBot)
            {
                state.Board[index] = userPiece; //Update the game state
                PlacePiece(index, userPiece); //Place the piece on the board
                turnUser = User.bot; //Hand over the turn
                uiHandler.SetTurnText(turnUser); //Display the turn text
                CheckBoardState(); //Check if there is a win

                Invoke(nameof(MakeAIMove), Random.Range(0.7f, 2.5f)); //Allow the bot make a move
            }
            else //Playing Against A Bot
            {
                XAndOMove move = new XAndOMove
                {
                    val = index
                };

                //Send move to server
                var result = await SessionHandler.MakeMove(GameManager.gameSession.sessionId.ToString(), move);
                result.State = Newtonsoft.Json.JsonConvert.DeserializeObject<XAndOState>(result.State.ToString());

                turnUser = User.bot; //Hand over the turn
                uiHandler.SetTurnText(turnUser); //Display the turn text
            }
        }
    }

    private void MakeAIMove()
    {
        if (isGameOver) return;

        int index = bot.ThinkMove(state.Board); //Let the bot think a move
        state.Board[index] = botPiece; //Update the game state
        PlacePiece(index, botPiece); //Place the bot piece on the board
        turnUser = User.host; //Hand over the turn
        uiHandler.SetTurnText(turnUser); //Display the turn text
        CheckBoardState(); //Check if there is a win
    }

    #region Board UI Update
    private void PlacePiece(int index, string type)
    {
        GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();
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

        state.Board = new string[9];
        System.Array.Fill(state.Board, "f");
    }
    #endregion

    #region Board Check
    private void CheckBoardState()
    {
        var win = CheckWinState(userPiece); //Check if player has won

        if (win != -1)
        {
            isGameOver = true;
            ActivateWinLine(win);
            GameManager.instance.GetComponent<AudioManager>().PlayVictorySound();
            uiHandler.DisplayWinScreen("You Have Won");
        }

        win = CheckWinState(botPiece); //Check if bot has won
        if (win != -1)
        {
            isGameOver = true;
            ActivateWinLine(win);
            GameManager.instance.GetComponent<AudioManager>().PlayDefeatSound();
            uiHandler.DisplayWinScreen("Bot Has Won");
        }

        //No one has won yet
        for (int i = 0; i < 9; i++)
        {
            if (state.Board[i] == "f") //There's an empty space
                return; //Game should keep on going (There's still free space)
        }

        //Game is a draw
        uiHandler.SetTurnText(turnUser, "Draw!!!");
        GameManager.instance.GetComponent<AudioManager>().PlayDrawSound();
        Invoke(nameof(ClearBoard), 1f);
    }

    /// <summary>Returns the direction of the win or -1 if there is no win</summary>
    private int CheckWinState(string piece)
    {
        //Check Columns
        if (state.Board[0] == piece && state.Board[1] == piece && state.Board[2] == piece)
        {
            return 1;
        }
        else if (state.Board[3] == piece && state.Board[4] == piece && state.Board[5] == piece)
        {
            return 2;
        }
        else if (state.Board[6] == piece && state.Board[7] == piece && state.Board[8] == piece)
        {
            return 3;
        }
        //Check Rows
        else if (state.Board[0] == piece && state.Board[3] == piece && state.Board[6] == piece)
        {
            return 4;
        }
        else if (state.Board[1] == piece && state.Board[4] == piece && state.Board[7] == piece)
        {
            return 5;
        }
        else if (state.Board[2] == piece && state.Board[5] == piece && state.Board[8] == piece)
        {
            return 6;
        }
        //Check Diagonals
        else if (state.Board[0] == piece && state.Board[4] == piece && state.Board[8] == piece)
        {
            return 7;
        }
        else if (state.Board[6] == piece && state.Board[4] == piece && state.Board[2] == piece)
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
