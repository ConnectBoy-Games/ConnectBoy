using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class DotsAndBoxesManager : MonoBehaviour, IGameManager
{
    private DotsAndBoxesState localState = new();
    private DotsAndBoxesBot bot;
    private User turnUser;

    private bool isGameOver = false;

    [Header("UI Handling")]
    [SerializeField] DotsAndBoxesUIHandler uiHandler;

    [Header("GameBoard Handling")]
    [SerializeField] Color tileDefault;
    [SerializeField] Color edgeColor;
    [SerializeField] Color tilePlayer1; //The color for the first player
    [SerializeField] Color tilePlayer2; //The color for the second player
    [SerializeField] Image[] boxes;
    [SerializeField] Image[] verticalButtons;
    [SerializeField] Image[] horizontalButtons;

    async void OnEnable() //The entry point of the Game Manager
    {
        isGameOver = false;
        ScorePanel.instance.UpdateScore(0, 0);
        ClearBoard();

        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                //Set who has the turn
                turnUser = (User)Random.Range(1, 3);
                uiHandler.SetTurnText(turnUser);

                //Set the bot difficulty
                bot = new DotsAndBoxesBot(GameManager.botDifficulty);

                //Make an AI move if it has the turn
                if (turnUser == User.bot) Invoke(nameof(MakeAIMove), 1f);
                break;
            case GameMode.vsPlayer:
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
                }
                else //The other player is the starting player
                {
                    turnUser = User.player;
                    uiHandler.SetTurnText(User.player);
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
                    Invoke(nameof(MakeAIMove), Random.Range(0.7f, 2.5f)); //Allow the bot make a move
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

    public async void MakeHorizontalMove(int move)
    {
        if (isGameOver || localState.HorizontalEdges.Contains(move))
        {
            Handheld.Vibrate();
            return;
        }

        if (GameManager.gameMode == GameMode.vsBot && turnUser == User.client)
        {
            localState.HorizontalEdges.Add(move);
            SetHorizontalButtonColor(move, turnUser);
            GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();

            if (CheckHorizontalBox(move) == false) //Retain the turn if a box was completed
            {
                SwitchTurns();
            }
        }
        else if (GameManager.gameMode == GameMode.vsPlayer)
        {
            localState.HorizontalEdges.Add(move);
            SetHorizontalButtonColor(move, turnUser);
            GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();

            //Retain the turn if a box was completed
            if (CheckHorizontalBox(move) == false)
            {
                SwitchTurns();
            }
        }
        else if (GameManager.gameMode == GameMode.online)
        {
            DaBMove mov = new DaBMove
            {
                H = move,
                V = -1
            };

            //Send move to server
            var result = await SessionHandler.MakeMove(GameManager.gameSession.sessionId.ToString(), move);
            var tempState = JsonConvert.DeserializeObject<DotsAndBoxesState>(JsonConvert.SerializeObject(result.State));

            ProcessState(tempState); //Process the game state
        }
        else
        {
            Handheld.Vibrate();
        }

        uiHandler.UpdateScoreUI(localState); //Update the score display
    }

    public bool CheckHorizontalBox(int id)
    {
        int topBoxId = id - 5;
        int bottomBoxId = id;
        bool top = false, bottom = false;

        if (topBoxId < 0) //We are the the top, only check bottom
        {
            bottom = CheckBox(bottomBoxId); //Check Bottom Box
        }
        else if (bottomBoxId > 29) //We are at the bottom, only check top 
        {
            top = CheckBox(topBoxId); //Check Top Box
        }
        else
        {
            top = CheckBox(topBoxId); //Check Top Box
            bottom = CheckBox(bottomBoxId); //Check Bottom Box
        }

        if (top) //We completed a top box
        {
            SetBoxColor(topBoxId, turnUser);
            localState.Boxes.Add(topBoxId); //Update the game state
            GameManager.instance.GetComponent<AudioManager>().PlayWobbleSound();
            if (turnUser == User.client)
            {
                localState.Player1Scores++;
            }
            else
            {
                localState.Player2Scores++;
            }
        }

        if (bottom) //We completed a bottom box
        {
            SetBoxColor(bottomBoxId, turnUser);
            localState.Boxes.Add(bottomBoxId); //Update the game state
            GameManager.instance.GetComponent<AudioManager>().PlayWobbleSound();
            if (turnUser == User.client)
            {
                localState.Player1Scores++;
            }
            else
            {
                localState.Player2Scores++;
            }
        }

        return (top || bottom);
    }

    public async void MakeVerticalMove(int move)
    {
        if (isGameOver || localState.VerticalEdges.Contains(move))
        {
            Handheld.Vibrate();
            return;
        }

        if (GameManager.gameMode == GameMode.vsBot && turnUser == User.client)
        {
            localState.VerticalEdges.Add(move);
            SetVerticalButtonColor(move, turnUser);
            GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();

            //Retain the turn if a box was completed
            if (CheckVerticalBox(move) == false)
            {
                SwitchTurns();
            }
        }
        else if (GameManager.gameMode == GameMode.vsPlayer)
        {
            localState.VerticalEdges.Add(move);
            SetVerticalButtonColor(move, turnUser);
            GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();

            //Retain the turn if a box was completed
            if (CheckVerticalBox(move) == false)
            {
                SwitchTurns();
            }
        }
        else if (GameManager.gameMode == GameMode.online)
        {
            DaBMove mov = new DaBMove
            {
                H = -1,
                V = move
            };

            //Send move to server
            var result = await SessionHandler.MakeMove(GameManager.gameSession.sessionId.ToString(), move);
            var tempState = JsonConvert.DeserializeObject<DotsAndBoxesState>(JsonConvert.SerializeObject(result.State));

            ProcessState(tempState); //Process the game state
        }
        else
        {
            Handheld.Vibrate();
        }

        uiHandler.UpdateScoreUI(localState); //Update the score display
    }

    public bool CheckVerticalBox(int id)
    {
        //Check Left & Right
        int leftBoxId = id - (int)(id / 6) - 1;
        int rightBoxId = leftBoxId + 1;
        bool left = false, right = false;

        if (leftBoxId % 6 == 0) //We are at the left, only check the right
        {
            right = CheckBox(leftBoxId); //Check Bottom Box
        }
        else if (rightBoxId > 29) //We are at the bottom, only check top 
        {
            left = CheckBox(leftBoxId); //Check Top Box
        }
        else
        {
            left = CheckBox(leftBoxId); //Check Top Box
            right = CheckBox(rightBoxId); //Check Bottom Box
        }

        if (left) //We completed a top box
        {
            SetBoxColor(leftBoxId, turnUser);
            localState.Boxes.Add(leftBoxId); //Update the game state
            GameManager.instance.GetComponent<AudioManager>().PlayWobbleSound();

            if (turnUser == User.client)
            {
                localState.Player1Scores++;
            }
            else
            {
                localState.Player2Scores++;
            }
        }

        if (right) //We completed a bottom box
        {
            SetBoxColor(rightBoxId, turnUser);
            localState.Boxes.Add(rightBoxId); //Update the game state
            GameManager.instance.GetComponent<AudioManager>().PlayWobbleSound();
            if (turnUser == User.client)
            {
                localState.Player1Scores++;
            }
            else
            {
                localState.Player2Scores++;
            }
        }

        return (left || right);
    }

    /// <summary>Retruns true if the box is completed</summary>
    /// <param name="id">ID of the box to be checked</param>
    public bool CheckBox(int id)
    {
        if (id < 0 || id >= 25) return false;

        int r = (int)(id / 5);

        // Correct indices based on the grid mapping:
        int left = id + r;
        int right = left + 1;

        return localState.HorizontalEdges.Contains(id) &&
               localState.HorizontalEdges.Contains(id + 5) &&
               localState.VerticalEdges.Contains(left) &&
               localState.VerticalEdges.Contains(right);
    }

    public void MakeAIMove()
    {
        if (isGameOver) return;

        DaBMove move = bot.ThinkMove(localState.HorizontalEdges, localState.VerticalEdges); //Let the bot think a move
        GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();

        if(move.H == -1) //It's a vertical move
        {
            localState.VerticalEdges.Add(move.V); //Update the game state
            SetVerticalButtonColor(move.V, turnUser); //Set the color of the button clicked

            //Retain the turn if a box was completed
            if (CheckVerticalBox(move.V) == false)
            {
                SwitchTurns();
            }
            else
            {
                Invoke(nameof(MakeAIMove), Random.Range(0.7f, 2.5f)); //Allow the bot make a move again
            }
        }
        else if (move.V == -1) //It's a horizontal move
        {
            localState.HorizontalEdges.Add(move.H); //Update the game state
            SetHorizontalButtonColor(move.H, turnUser); //Set the color of the button clicked

            //Retain the turn if a box was completed
            if (CheckHorizontalBox(move.H) == false)
            {
                SwitchTurns();
            }
            else
            {
                Invoke(nameof(MakeAIMove), Random.Range(0.7f, 2.5f)); //Allow the bot make a move again
            }
        }
        else
        {
            NotificationDisplay.instance.DisplayMessage("The DAB has run into a critical error!", NotificationType.error);
        }
        
        uiHandler.UpdateScoreUI(localState); //Update the score display
    }

    public void CheckBoardState()
    {

    }

    public void SetHorizontalButtonColor(int button, User user)
    {
        if (user == User.client)
        {
            horizontalButtons[button].CrossFadeColor(edgeColor, 1f, true, false);
        }
        else
        {
            horizontalButtons[button].CrossFadeColor(edgeColor, 1f, true, false);
        }
    }

    public void SetVerticalButtonColor(int button, User user)
    {
        if (user == User.client)
        {
            verticalButtons[button].CrossFadeColor(edgeColor, 1f, true, false);
        }
        else
        {
            verticalButtons[button].CrossFadeColor(edgeColor, 1f, true, false);
        }
    }

    public void SetBoxColor(int box, User user)
    {
        boxes[box].CrossFadeAlpha(1, 0.1f, true);

        if (user == User.client)
        {
            boxes[box].CrossFadeColor(tilePlayer1, 1f, true, false);
        }
        else
        {
            boxes[box].CrossFadeColor(tilePlayer2, 1f, true, false);
        }
    }

    public void ClearBoard()
    {
        localState.HorizontalEdges = new();
        localState.VerticalEdges = new();
        localState.Boxes = new();
    }

    public int CheckWinState(string piece)
    {
        throw new System.NotImplementedException();
    }

    private void ProcessState(DotsAndBoxesState state)
    {

    }

    public async void GetGameState()
    {
        try
        {
            var result = await SessionHandler.GetSessionGameState(GameManager.gameSession.sessionId.ToString());
            var tempState = JsonConvert.DeserializeObject<DotsAndBoxesState>(JsonConvert.SerializeObject(result));

            ProcessState(tempState);
            Invoke(nameof(GetGameState), 5f); //Call itself again after 5 seconds
        }
        catch (System.Exception e)
        {
            NotificationDisplay.instance.DisplayMessage("Error getting the game state from the server: " + e.Message, NotificationType.error);
        }
    }
}

