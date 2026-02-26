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
        ClearBoard();

        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (User)Random.Range(1, 3); //Set who has the turn
                uiHandler.SetTurnText(turnUser);

                bot = new DotsAndBoxesBot(GameManager.botDifficulty); //Set the bot difficulty
                if (turnUser == User.bot) Invoke(nameof(MakeAIMove), 1f); //Make an AI move if it has the turn
                break;
            case GameMode.vsPlayer:
                ScorePanel.instance.SetUsernames("Player 1", "Player 2");
                turnUser = (User)Random.Range(2, 4); //Set who has the turn
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

                //If it is the bot's turn, allow it to make a move
                if (turnUser == User.bot) Invoke(nameof(MakeAIMove), Random.Range(0.7f, 2.5f));
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

        CheckBoardState();
    }

    public bool CheckHorizontalBox(int id)
    {
        int topBoxId = id - 5;
        int bottomBoxId = id;
        bool top, bottom;

        top = CheckBox(topBoxId); //Check Top Box
        bottom = CheckBox(bottomBoxId); //Check Bottom Box

        if (top) //We completed a top box
        {
            SetBoxColor(topBoxId, turnUser);
            localState.Boxes.Add(topBoxId); //Update the game state
            GameManager.instance.GetComponent<AudioManager>().PlayWobbleSound();

            //Update scores
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

            //Update scores
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

        CheckBoardState();
    }

    public bool CheckVerticalBox(int id)
    {
        //Check Left & Right
        int rightBoxId = id - (int)(id / 6);
        int leftBoxId = rightBoxId - 1;
        bool left = false, right = false;

        if (id % 6 == 0)//We are at the leftmost side(Check only the right box)
        {
            right = CheckBox(rightBoxId); //Check Right Box
        }
        else if ((id - 5) % 6 == 0) //We are at the rightmost side (Check only left box)
        {
            left = CheckBox(leftBoxId); //Check Left Box
        }
        else
        {
            right = CheckBox(rightBoxId); //Check Right Box
            left = CheckBox(leftBoxId); //Check Left Box
        }

        if (left) //We completed a top box
        {
            SetBoxColor(leftBoxId, turnUser);
            localState.Boxes.Add(leftBoxId); //Update the game state
            GameManager.instance.GetComponent<AudioManager>().PlayWobbleSound();

            //Update scores
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

            //Update scores
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

    /// <summary>Returns true if the box is completed</summary>
    /// <param name="id">ID of the box to be checked</param>
    public bool CheckBox(int id)
    {
        if (id < 0 || id >= 25 || localState.Boxes.Contains(id)) return false; //Do bounds checking

        int r = (int)(id / 5);

        return localState.HorizontalEdges.Contains(id) && localState.HorizontalEdges.Contains(id + 5) && //Top && Bottom
               localState.VerticalEdges.Contains(id + r) && localState.VerticalEdges.Contains(id + r + 1); //Left && Right
    }

    public void MakeAIMove()
    {
        if (isGameOver) return;

        DaBMove move = bot.ThinkMove(localState.HorizontalEdges, localState.VerticalEdges); //Let the bot think a move
        GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();

        if (move.H == -1) //It's a vertical move
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
                Invoke(nameof(MakeAIMove), Random.Range(0.7f, 1.5f)); //Allow the bot make a move again
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
                Invoke(nameof(MakeAIMove), Random.Range(0.7f, 1.5f)); //Allow the bot make a move again
            }
        }
        else
        {
            NotificationDisplay.instance.DisplayMessage("The DAB has run into a critical error!", NotificationType.error);
        }

        CheckBoardState();
    }

    public void CheckBoardState()
    {
        uiHandler.UpdateScoreUI(localState);
        
        if (localState.Player1Scores + localState.Player2Scores >= 25)
        {
            isGameOver = true;
            GameManager.instance.GetComponent<AudioManager>().PlayVictorySound();

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
        isGameOver = false;
        ScorePanel.instance.UpdateScore(0, 0);

        localState.HorizontalEdges = new();
        localState.VerticalEdges = new();
        localState.Boxes = new();
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