using UnityEngine;

public class FourInOneManager : MonoBehaviour
{
    //Internal Variables
    public static Wagr.Session gameSession;
    public static GameMode gameMode;

    private User turnUser; //Who has the turn?

    private int userPiece; //Either a 1 or 2 (X or O)
    private int botPiece; //Either a 1 or 2 (X or O)

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
    public bool MakeMove(int column)
    {
        // 1. Check if column is valid and not full
        if (column < 0 || column >= Cols || board[0, column] != 0)
            return false;

        // 2. Find the lowest empty row (starting from the bottom)
        for (int r = Rows - 1; r >= 0; r--)
        {
            if (board[r, column] == 0)
            {
                board[r, column] = currentPlayer;

                // 3. Check for win immediately after placing
                if (CheckWin(r, column))
                {
                    Debug.Log($"Player {currentPlayer} Wins!");
                }
                else
                {
                    currentPlayer = (currentPlayer == 1) ? 2 : 1;
                }
                return true;
            }
        }
        return false;
    }

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
