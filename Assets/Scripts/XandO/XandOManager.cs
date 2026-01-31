using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class XandOManager : MonoBehaviour
{
    private XandOBot bot; //X And O bot
    private User turnUser; //Who has the turn?

    private string userPiece; // (x or o)
    private string botPiece; // (x or o)
    private string otherPlayerId;
    private bool isGameOver = false;

    private XAndOState localState = new();
    public SessionDetails det;

    [SerializeField] XandOUIHandler uiHandler;

    [Header("UI Handling")]
    [SerializeField] private GameObject imageX; //0
    [SerializeField] private GameObject imageO; //1
    [SerializeField] private List<GameObject> buttons = new();
    [SerializeField] private List<GameObject> winLines = new();

    public async void Start()
    {
        localState.Board = new string[9];
        System.Array.Fill(localState.Board, "f");

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
        else
        {
            det = await SessionHandler.CheckSessionStatus(GameManager.gameSession.sessionId.ToString()); //Get the session details

            if(GameManager.gameSession.gameRole == GameRole.host)
            {
                otherPlayerId = det.OtherPlayer.Id;
            }
            else
            {
                otherPlayerId = det.HostPlayer.Id;
            }

            if (det.CurrentTurn == GameManager.instance.accountManager.playerProfile.Id) //You are the starting player
            {
                turnUser = User.client;
                uiHandler.SetTurnText(User.client);
                userPiece = "x";
                botPiece = "o";
            }
            else //The other player is the starting player
            {
                turnUser = User.player;
                uiHandler.SetTurnText(User.player);
                userPiece = "o";
                botPiece = "x";
            }
            Invoke(nameof(GetGameState), 5f);
        }
    }

    //For allowing the player to make a move
    public async void MakeMove(int index)
    {
        if (localState.Board[index] == "f" && turnUser == User.client && !isGameOver) //Valid move
        {
            if (GameManager.gameSession.gameMode == GameMode.vsBot)
            {
                localState.Board[index] = userPiece; //Update the game state
                PlacePiece(index, userPiece, true); //Place the piece on the board
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
                var tempState = JsonConvert.DeserializeObject<XAndOState>(JsonConvert.SerializeObject(result.State));

                ProcessState(tempState); //Process the game state
                turnUser = User.player; //Hand over the turn
                uiHandler.SetTurnText(turnUser); //Display the turn text
            }
        }
    }

    private void MakeAIMove()
    {
        if (isGameOver) return;

        int index = bot.ThinkMove(localState.Board); //Let the bot think a move
        localState.Board[index] = botPiece; //Update the game state
        PlacePiece(index, botPiece, true); //Place the bot piece on the board
        turnUser = User.client; //Hand over the turn
        uiHandler.SetTurnText(turnUser); //Display the turn text
        CheckBoardState(); //Check if there is a win
    }

    #region Board UI Update
    private void PlacePiece(int index, string type, bool sound = false)
    {
        if (sound) GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();

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

        localState.Board = new string[9];
        System.Array.Fill(localState.Board, "f");
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
            uiHandler.DisplayWinScreen("You Have Won ", GameManager.gameSession.wager);
        }

        win = CheckWinState(botPiece); //Check if bot has won
        if (win != -1)
        {
            isGameOver = true;
            ActivateWinLine(win);
            GameManager.instance.GetComponent<AudioManager>().PlayDefeatSound();
            uiHandler.DisplayDefeatScreen("You Have Lost ", GameManager.gameSession.wager);
        }

        //No one has won yet
        for (int i = 0; i < 9; i++)
        {
            if (localState.Board[i] == "f") //There's an empty space
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
        if (localState.Board[0] == piece && localState.Board[1] == piece && localState.Board[2] == piece)
        {
            return 1;
        }
        else if (localState.Board[3] == piece && localState.Board[4] == piece && localState.Board[5] == piece)
        {
            return 2;
        }
        else if (localState.Board[6] == piece && localState.Board[7] == piece && localState.Board[8] == piece)
        {
            return 3;
        }
        //Check Rows
        else if (localState.Board[0] == piece && localState.Board[3] == piece && localState.Board[6] == piece)
        {
            return 4;
        }
        else if (localState.Board[1] == piece && localState.Board[4] == piece && localState.Board[7] == piece)
        {
            return 5;
        }
        else if (localState.Board[2] == piece && localState.Board[5] == piece && localState.Board[8] == piece)
        {
            return 6;
        }
        //Check Diagonals
        else if (localState.Board[0] == piece && localState.Board[4] == piece && localState.Board[8] == piece)
        {
            return 7;
        }
        else if (localState.Board[6] == piece && localState.Board[4] == piece && localState.Board[2] == piece)
        {
            return 8;
        }
        else
        {
            return -1;
        }
    }
    #endregion

    #region Server Update
    void ProcessState(XAndOState state)
    {
        ClearBoard();

        for(int i = 0; i < state.Board.Length; i++)
        {
            if (state.Board[i] == GameManager.instance.accountManager.playerProfile.Id)
            {
                localState.Board[i] = userPiece;
                PlacePiece(i, userPiece);
            }
            else if(state.Board[i] == otherPlayerId)
            {
                localState.Board[i] = botPiece;
                PlacePiece(i, botPiece);
            }
            else
            {
                localState.Board[i] = "f";
            }
            CheckBoardState();
        }
    }

    public async void GetGameState()
    {
        print("Getting game State");
        var result = await SessionHandler.GetSessionGameState(GameManager.gameSession.sessionId.ToString());
        var tempState = JsonConvert.DeserializeObject<XAndOState>(JsonConvert.SerializeObject(result));

        print("Got game State");
        ProcessState(tempState);
        Invoke(nameof(GetGameState), 5f); //Call itself again after 5 seconds
    }
    #endregion
}
