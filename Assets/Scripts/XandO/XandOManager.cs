using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class XandOManager : MonoBehaviour, IGameManager
{
    private XAndOState localState = new();
    private User turnUser; //Who has the turn?
    private XandOBot bot; //X And O bot

    private string userPiece; // (x or o)
    private string otherPiece; // (x or o)
    private bool isGameOver = false;
    private bool isPaused = false;

    [Header("UI Handling")]
    [SerializeField] XandOUIHandler uiHandler;

    [Header("GameBoard Handling")]
    [SerializeField] private GameObject imageX; //0
    [SerializeField] private GameObject imageO; //1
    [SerializeField] private List<GameObject> buttons = new();
    [SerializeField] private List<GameObject> winLines = new();

    async void OnEnable() //The entry point of the Game Manager
    {
        isGameOver = false;
        ScorePanel.instance.UpdateScore(0, 0);
        ClearBoard();

        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                //Set who gets which piece
                userPiece = (Random.Range(0, 2) == 1) ? "x" : "o";
                otherPiece = (userPiece == "x") ? "o" : "x"; //Set the alternative piece

                turnUser = (User)Random.Range(1, 3); //Set who has the turn
                uiHandler.SetTurnText(turnUser);

                //Set the bot difficulty
                bot = new XandOBot(GameManager.botDifficulty, userPiece, otherPiece);

                //Make an AI move if it has the turn
                if (turnUser == User.bot) Invoke(nameof(MakeAIMove), 0.5f);
                break;
            case GameMode.vsPlayer:
                ScorePanel.instance.SetUsernames("Player 1", "Player 2");

                //Set who gets which piece
                userPiece = (Random.Range(0, 2) == 1) ? "x" : "o";
                otherPiece = (userPiece == "x") ? "o" : "x"; //Set the alternative piece

                //Set who has the turn
                turnUser = (User)Random.Range(2, 4);
                uiHandler.SetTurnText(turnUser);
                break;
            case GameMode.online:
                var det = await SessionHandler.CheckSessionStatus(GameManager.gameSession.sessionId.ToString()); //Get the session details

                if (det.CurrentTurn == GameManager.instance.accountManager.playerProfile.Id) //You are the starting player
                {
                    turnUser = User.client;
                    uiHandler.SetTurnText(User.client);
                    userPiece = "x";
                    otherPiece = "o";
                }
                else //The other player is the starting player
                {
                    turnUser = User.player;
                    uiHandler.SetTurnText(User.player);
                    userPiece = "o";
                    otherPiece = "x";
                }
                Invoke(nameof(GetGameState), 5f);
                break;
        }
    }

    public void ClearBoard()
    {
        //Clear what the buttons are holding
        foreach (GameObject button in buttons)
        {
            foreach (Transform piece in button.transform)
            {
                Destroy(piece.gameObject);
            }
        }

        //Disable the lines in the game
        foreach (GameObject line in winLines)
        {
            line.SetActive(false);
        }

        localState.Board = new string[9];
        System.Array.Fill(localState.Board, "f");
    }

    public void SwitchTurns()
    {
        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (turnUser == User.bot) ? User.client : User.bot;

                if (turnUser == User.bot) Invoke(nameof(MakeAIMove), Random.Range(0.7f, 1.5f)); //Allow the bot make a move
                break;
            case GameMode.vsPlayer:
                turnUser = (turnUser == User.client) ? User.player : User.client;
                break;
            case GameMode.online:
                turnUser = (turnUser == User.client) ? User.player : User.client;
                break;
        }
        isPaused = false;
        uiHandler.SetTurnText(turnUser); //Display the turn text
    }

    public async void MakeMove(int index)//For allowing the player to make a move
    {
        if (!isGameOver && !isPaused && localState.Board[index] == "f") //Valid move
        {
            if (GameManager.gameMode == GameMode.vsBot && turnUser == User.client)
            {
                localState.Board[index] = userPiece; //Update the game state
                PlacePiece(index, userPiece); //Place the piece on the board
                GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();

                CheckBoardState(); //Check if there is a win
                SwitchTurns(); //Hand over the turn
            }
            else if (GameManager.gameMode == GameMode.vsPlayer) //Local Player
            {
                switch (turnUser) //Player1
                {
                    case User.client:
                        localState.Board[index] = userPiece; //Update the game state
                        PlacePiece(index, userPiece); //Place the piece on the board
                        break;
                    case User.player:
                        localState.Board[index] = otherPiece; //Update the game state
                        PlacePiece(index, otherPiece); //Place the piece on the board
                        break;
                    default:
                        Debug.LogError("Bot should not be active in PvP mode!");
                        NotificationDisplay.instance.DisplayMessage("Bot should not be active in PvP mode!", NotificationType.error);
                        break;
                }

                GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();
                CheckBoardState(); //Check if there is a win
                SwitchTurns(); //Hand over the turn
            }
            else if (GameManager.gameMode == GameMode.vsPlayer && turnUser == User.client) //Playing Online
            {
                XAndOMove move = new XAndOMove
                {
                    val = index
                };

                //Send move to server
                var result = await SessionHandler.MakeMove(GameManager.gameSession.sessionId.ToString(), move);
                var tempState = JsonConvert.DeserializeObject<XAndOState>(JsonConvert.SerializeObject(result.State));

                ProcessState(tempState); //Process the game state
            }
            else
            {
                Handheld.Vibrate();
            }
        }
        else
        {
            Handheld.Vibrate();
        }
    }

    private void MakeAIMove()
    {
        if (isGameOver || isPaused) return;

        int index = bot.ThinkMove(localState.Board); //Let the bot think a move
        localState.Board[index] = otherPiece; //Update the game state
        PlacePiece(index, otherPiece); //Place the bot piece on the board
        GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();
        CheckBoardState(); //Check if there is a win
        SwitchTurns();
    }

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
        winLines[index].SetActive(true);
    }

    public void CheckBoardState()
    {
        isPaused = true;

        int win = CheckWinState(userPiece); //Check if player has won
        if (win != -1)
        {
            ActivateWinLine(win);
            localState.Player1Scores++;
            GameManager.instance.GetComponent<AudioManager>().PlayVictorySound();
            ScorePanel.instance.UpdateScore(localState.Player1Scores, localState.Player2Scores);
        }

        win = CheckWinState(otherPiece); //Check if bot or other player has won
        if (win != -1)
        {
            ActivateWinLine(win);
            localState.Player2Scores++;
            GameManager.instance.GetComponent<AudioManager>().PlayVictorySound();
            ScorePanel.instance.UpdateScore(localState.Player1Scores, localState.Player2Scores);
        }

        if (localState.Player1Scores >= 3 || localState.Player2Scores >= 3) //Playing a best of 3 rounds
        {
            isGameOver = true;
        }
        else //Game has been won but not the match, clear the board for the next round
        {
            Invoke(nameof(ClearBoard), 0.7f);
        }

        if (isGameOver)
        {
            switch (GameManager.gameMode)
            {
                case GameMode.vsBot:
                    if (localState.Player1Scores > localState.Player2Scores)
                    {
                        localState.Winner = User.client.ToString();
                        uiHandler.DisplayWinScreen("Player 1 wins the game!");
                    }
                    else
                    {
                        localState.Winner = User.bot.ToString();
                        uiHandler.DisplayDefeatScreen("You lost the game!");
                    }
                    break;
                case GameMode.vsPlayer:
                    if (localState.Player1Scores > localState.Player2Scores)
                    {
                        localState.Winner = User.client.ToString();
                        uiHandler.DisplayWinScreen("Player 1 wins the game!");
                    }
                    else
                    {
                        localState.Winner = User.player.ToString();
                        uiHandler.DisplayWinScreen("Player 2 wins the game!");
                    }
                    break;
                case GameMode.online:
                    break;
            }
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
    public int CheckWinState(string piece)
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

    void ProcessState(XAndOState state)
    {
        ClearBoard();
        string otherPlayerId = GameManager.gameSession.other.Id;

        for (int i = 0; i < state.Board.Length; i++)
        {
            if (state.Board[i] == GameManager.instance.accountManager.playerProfile.Id)
            {
                localState.Board[i] = userPiece;
                PlacePiece(i, userPiece);
            }
            else if (state.Board[i] == otherPlayerId)
            {
                localState.Board[i] = otherPiece;
                PlacePiece(i, otherPiece);
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
        try
        {
            print("Getting game State");
            var result = await SessionHandler.GetSessionGameState(GameManager.gameSession.sessionId.ToString());
            var tempState = JsonConvert.DeserializeObject<XAndOState>(JsonConvert.SerializeObject(result));

            print("Got game State");
            ProcessState(tempState);
            Invoke(nameof(GetGameState), 5f); //Call itself again after 5 seconds

        }
        catch (System.Exception ex)
        {
            NotificationDisplay.instance.DisplayMessage("Failed to get game state, retrying " + ex.Message, NotificationType.error);
        }
    }
}
