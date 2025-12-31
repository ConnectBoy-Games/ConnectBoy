using UnityEngine;

public class FourInOneManager : MonoBehaviour
{
    //Internal Variables
    public static Wagr.Session gameSession;
    public static GameMode gameMode;

    private User turnUser; //Who has the turn?

    private int userPiece; //Either a 1 or 2 (Red or Blue)
    private int botPiece; //Either a 1 or 2 (Red or Blue)
    private FourInOneBot bot;

    [SerializeField] FourInOneUIHandler uiHandler;

    //Game State Variable
    private const int Rows = 6;
    private const int Cols = 7;
    private int[,] board = new int[Rows, Cols];
    private int currentPlayer = 1;

    private GameObject[] columns;
    [SerializeField] GameObject redPiece, bluePiece;

    void Start()
    {
        ClearBoard();


    }

    // Call this when a user clicks a column button
    public void MakeMove(int column)
    {
        // 1. Check if column is valid and not full and it is the player's turn
        if (column < 0 || column >= Cols || board[0, column] != 0 && turnUser != User.host)
            return;

        if (gameMode == GameMode.vsPlayer)
        {
            //TODO: Send move to server
            SendMoveToServer();
        }
        else //Playing Against A Bot
        {
            // 2. Find the lowest empty row (starting from the bottom)
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (board[r, column] == 0) //It is empty
                {
                    board[r, column] = userPiece; //Update the game state
                    PlacePiece(column, userPiece); //Place the piece on the board

                    // 3. Check for win immediately after placing
                    if (CheckWin(r, column))
                    {
                        Debug.Log($"Player {currentPlayer} Wins!");
                    }
                    else
                    {
                        turnUser = User.bot;
                        uiHandler.SetTurnText(turnUser);
                        Invoke(nameof(MakeAIMove), Random.Range(0.7f, 2.5f)); //Allow the bot make a move
                    }
                }
            }
        }
    }

    #region AI Handling Functions
    private bool CheckWin(int row, int col)
    {
        int player = board[row, col];

        // Directions: {rowDelta, colDelta}
        int[,] directions = { { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 } };

        for (int i = 0; i < 4; i++)
        {
            int dr = directions[i, 0];
            int dc = directions[i, 1];
            int count = 1; // Count the piece just placed

            // Check both ways along the line (e.g., left and right)
            count += CountInDirection(row, col, dr, dc, player);
            count += CountInDirection(row, col, -dr, -dc, player);

            if (count >= 4) return true;
        }
        return false;
    }

    private int CountInDirection(int row, int col, int dr, int dc, int player)
    {
        int count = 0;
        int r = row + dr;
        int c = col + dc;

        // Move in the direction as long as we find the same player's piece
        while (r >= 0 && r < Rows && c >= 0 && c < Cols && board[r, c] == player)
        {
            count++;
            r += dr;
            c += dc;
        }
        return count;
    }

    private void MakeAIMove()
    {
        //Selects which column to place the chip and updates the game state
        int index = bot.ThinkMove(board); 
        PlacePiece(index, botPiece); //Place the piece on the board
        int row = 0;

        for (int r = Rows - 1; r >= 0; r--) //Get the row just played
        {
            if(board[r, index] == 0)
            {
                row = r + 1;
                break;
            }
        }
        
        if(CheckWin(row, index) == true)
        {
            //Bot has won
        }
        else
        {
            turnUser = User.host;
            uiHandler.SetTurnText(turnUser);
        }
    }
    #endregion

    #region Board Update
    private void PlacePiece(int colIndex, int pieceType)
    {
        switch (pieceType)
        {
            case 1:
                Instantiate(redPiece, columns[colIndex].transform);
                break;
            case 2:
                Instantiate(bluePiece, columns[colIndex].transform);
                break;
        }
        //TODO: Drag the component down to the right locaton
    }

    private void ClearBoard()
    {
        foreach (GameObject c in columns)
        {
            for (int i = 0; i < c.transform.childCount; i++)
            {
                Destroy(c.transform.GetChild(i).gameObject);
            }
        }

        board = new int[Rows, Cols];
    }

    #endregion

    #region Server Functions
    private void ConnectToServer()
    {

    }
    private void SendChatToServer()
    {

    }

    private void SendMoveToServer()
    {

    }
    #endregion
}
