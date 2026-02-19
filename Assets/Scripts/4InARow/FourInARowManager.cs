using Newtonsoft.Json;
using UnityEngine;

public class FourInARowManager : MonoBehaviour, IGameManager
{
    private FourInARowState localState = new();
    private User turnUser; //Who has the turn?
    private FourInARowBot bot;

    private int userPiece; //Either a 1 or 2 (Red or Blue)
    private int otherPiece; //Either a 1 or 2 (Red or Blue)

    //Game State Variable
    private const int Rows = 6;
    private const int Cols = 7;

    private bool isGameOver;

    [Header("UI Handling")]
    [SerializeField] FourInARowUIHandler uiHandler;

    [Header("GameBoard Handling")]
    [SerializeField] private GameObject[] columns;
    [SerializeField] GameObject redPiece, bluePiece;

    async void OnEnable()
    {
        isGameOver = false;
        ScorePanel.instance.DisableScore();
        ClearBoard();

        //*Temporary for testing
        GameManager.gameMode = GameMode.vsBot;
        turnUser = User.client; //Player starts first by default
        userPiece = 0; //Red piece
        otherPiece = 1; //Blue piece
        /*///*///End of temporary

        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                //Set who gets which piece
                userPiece = (Random.Range(0, 2) == 1) ? 1 : 0;
                otherPiece = (userPiece == 1) ? 0 : 1; //Set the alternative piece

                //Set who has the turn
                turnUser = (User)Random.Range(1, 3); //1-bot, 2-client
                uiHandler.SetTurnText(turnUser);

                //Initialize the bot with the selected difficulty
                bot = new FourInARowBot(GameManager.botDifficulty, userPiece, otherPiece); 

                //Make an AI move if it has the turn
                if (turnUser == User.bot) Invoke(nameof(MakeAIMove), Random.Range(0.7f, 1f));
                break;
            case GameMode.vsPlayer:
                //Set who gets which piece
                userPiece = (Random.Range(0, 2) == 1) ? 1 : 0;
                otherPiece = (userPiece == 1) ? 0 : 1; //Set the alternative piece

                //Set who has the turn
                turnUser = (User)Random.Range(2, 4); //2-player1, 3-player2
                uiHandler.SetTurnText(turnUser);
                break;
            case GameMode.online:
                var det = await SessionHandler.CheckSessionStatus(GameManager.gameSession.sessionId.ToString()); //Get the session details

                if (det.CurrentTurn == GameManager.instance.accountManager.playerProfile.Id) //You are the starting player
                {
                    turnUser = User.client;
                    uiHandler.SetTurnText(User.client);
                    userPiece = 1;
                    otherPiece = 0;
                }
                else //The other player is the starting player
                {
                    turnUser = User.player;
                    uiHandler.SetTurnText(User.player);
                    userPiece = 0;
                    otherPiece = 1;
                }
                Invoke(nameof(GetGameState), 5f);
                break;
        }
    }

    public void SwitchTurns()
    {
        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (turnUser == User.bot) ? User.client : User.bot;

                if (turnUser == User.bot) //If it is the bot's turn, allow it to make a move
                {
                    Invoke(nameof(MakeAIMove), Random.Range(0.7f, 2.5f));
                }
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

    public async void MakeMove(int column)
    {
        // Make sure r does not exceed the board limits
        int row = GetLowestRow(column);

        // Check if column is valid and not full and it is the player's turn
        if (column < 0 || column >= Cols || localState.Board[0, column] != 0 && turnUser != User.client || row < 0)
        {
            Handheld.Vibrate();
            return;
        }

        if (GameManager.gameMode == GameMode.vsBot && turnUser == User.client) // Playing against a bot and its the player's turn
        {
            localState.Board[row, column] = (int)turnUser; //Update the game state
            PlacePiece(row, column, userPiece); //Place the piece on the board
            GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();

            CheckBoardState(row, column); //Check if there is a win
            SwitchTurns(); //Hand over the turn and allow the bot make a move
        }
        else if (GameManager.gameMode == GameMode.vsPlayer) // 2 Player Mode
        {
            localState.Board[row, column] = (int)turnUser; //Update the game state
            switch (turnUser)
            {
                case User.client:
                    PlacePiece(row, column, userPiece); //Place the piece on the board
                    break;
                case User.player:
                    PlacePiece(row, column, otherPiece); //Place the piece on the board
                    break;
                default:
                    NotificationDisplay.instance.DisplayMessage("Bot should not be active in PvP mode!", NotificationType.error);
                    break;
            }

            GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();
            CheckBoardState(row, column); //Check if there is a win
            SwitchTurns(); //Hand over the turn
        }
        else if (GameManager.gameMode == GameMode.online)
        {
            FourInARowMove move = new FourInARowMove
            {
                col = column
            };

            //Send Move to server
            var result = await SessionHandler.MakeMove(GameManager.gameSession.sessionId.ToString(), move);
            var tempState = JsonConvert.DeserializeObject<FourInARowState>(JsonConvert.SerializeObject(result.State));

            ProcessState(tempState); //Process the game state
        }
    }

    private void MakeAIMove()
    {
        if (isGameOver) return;

        int column = bot.ThinkMove(localState.Board); //Selects which column to place the chip and updates the game state
        int row = GetLowestRow(column); //Find the lowest empty row in order to update the game state (starting from the bottom)
        
        if (localState.Board[row, column] == 0) //It is empty
        {
            localState.Board[row, column] = (int)turnUser; //Update the game state
            PlacePiece(row, column, otherPiece); //Place the bot piece on the board
            GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();

            CheckBoardState(row, column); //Check if there is a win
            SwitchTurns(); //Hand over the turn
        }
        else
        {
            NotificationDisplay.instance.DisplayMessage("The Bot has an error", NotificationType.error);
        }
    }

    private int GetLowestRow(int column) // Find the lowest empty row (starting from the bottom)
    {
        if (localState.Board[0, column] != 0) return -1; //Column is full

        int r;
        for (r = Rows - 1; r >= 0; r--)
        {
            if (localState.Board[r, column] == 0) //It is empty
                break;
        }
        return r;
    }

    private bool CheckWin(int row, int col, User turnUser)
    {
        // The +1 is to count the piece just placed

        //Count Horizontally
        int hor = 1 + CountInDirection(row, col, 0, 1, turnUser) + CountInDirection(row, col, 0, -1, turnUser);

        //Vertically
        int ver = 1 + CountInDirection(row, col, 1, 0, turnUser) + CountInDirection(row, col, -1, 0, turnUser);

        //Diagonal (top-left to bottom-right),
        int diag1 = 1 + CountInDirection(row, col, 1, 1, turnUser) + CountInDirection(row, col, -1, -1, turnUser);

        //Diagonal (top-right to bottom-left)
        int diag2 = 1 + CountInDirection(row, col, 1, -1, turnUser) + CountInDirection(row, col, -1, 1, turnUser);

        if (hor >= 4 || ver >= 4 || diag1 >= 4 || diag2 >= 4)
        {
            return true;
        }
        return false;
    }

    /// <summary>Counts the number of the same pieces in a direction and returns the count!</summary>
    private int CountInDirection(int row, int col, int dr, int dc, User user)
    {
        //(row,col) is the starting point, (dr, dc) is the direction to move in,
        int count = 0;
        int r = row + dr;
        int c = col + dc;

        // Move in the direction as long as we find the same player's piece
        while (r >= 0 && r < Rows && c >= 0 && c < Cols && localState.Board[r, c] == (int)user)
        {
            count++;
            r += dr;
            c += dc;
        }
        return count;
    }

    private void PlacePiece(int row, int colIndex, int pieceType)
    {
        FourInARowPiece piece;
        float ogY = -33.5f + (Rows * 14);
        float y = -33.5f + ((5 - row) * 14);

        switch (pieceType)
        {
            case 0:
                var temp = Instantiate(redPiece, new Vector3(columns[colIndex].transform.position.x, ogY, columns[colIndex].transform.position.z), Quaternion.identity, columns[colIndex].transform);
                piece = temp.GetComponent<FourInARowPiece>();
                piece.PlacePiece(new Vector3(columns[colIndex].transform.position.x, y, columns[colIndex].transform.position.z));
                break;
            case 1:
                temp = Instantiate(bluePiece, new Vector3(columns[colIndex].transform.position.x, ogY, columns[colIndex].transform.position.z), Quaternion.identity, columns[colIndex].transform);
                piece = temp.GetComponent<FourInARowPiece>();
                piece.PlacePiece(new Vector3(columns[colIndex].transform.position.x, y, columns[colIndex].transform.position.z));
                break;
        }
    }

    public void ClearBoard()
    {
        foreach (GameObject c in columns)
        {
            foreach (Transform item in c.transform)
            {
                Destroy(item.gameObject);
            }
        }
        localState.Board = new int[Rows, Cols];
    }

    public void CheckBoardState(int row, int col)
    {
        Debug.Log(turnUser.ToString() + CheckWin(row, col, turnUser));
    }

    public async void GetGameState()
    {
        try
        {
            var result = await SessionHandler.GetSessionGameState(GameManager.gameSession.sessionId.ToString());
            var tempState = JsonConvert.DeserializeObject<FourInARowState>(JsonConvert.SerializeObject(result));

            ProcessState(tempState);
            Invoke(nameof(GetGameState), 5f); //Call itself again after 5 seconds
        }
        catch (System.Exception e)
        {
            NotificationDisplay.instance.DisplayMessage("Error getting the game state: " + e.Message, NotificationType.error);
        }
    }

    public void ProcessState(FourInARowState state)
    {
        //TODO: Implement this
        ClearBoard();
        string otherPlayerId = GameManager.gameSession.other.Id;

        /*
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
            //CheckBoardState();
        }
        */
    }
}
