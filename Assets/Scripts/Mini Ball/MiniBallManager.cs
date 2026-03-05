using Newtonsoft.Json;
using UnityEngine;

public class MiniBallManager : MonoBehaviour, IGameManager
{
    private MiniBallState localState = new();
    private MiniBallBot bot; //Reference to the game bot
    private User turnUser; //Who has the turn

    public bool isGameOver = false;
    public bool isPaused = false;

    [Header("UI Handling")]
    [SerializeField] private MiniBallUIHandler uiHandler;

    [Header("Game Handling")]
    [SerializeField] private Transform ball;
    [SerializeField] TeamManager team1; //The parent object that holds the player 1 pieces
    [SerializeField] TeamManager team2; //The parent object that holds the player 2 pieces

    async void OnEnable() //The entry point of the Game Manager
    {
        ClearBoard();

        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (User)Random.Range(1, 3); //Set who has the turn
                uiHandler.SetTurnText(turnUser);

                bot = new MiniBallBot(); //Set the bot difficulty
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

    //Called after the player(turnUser) has made a move
    public async void MadeMove(MiniBallMove move)
    {

        //Wait for a couple of seconds and then process the game 
    }

    public void MakeAIMove()
    {
        var move = bot.MakeMove(localState); //Make move

        MadeMove(move); //Report the move that was made
    }

    public void SwitchTurns()
    {
        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (turnUser == User.bot) ? User.client : User.bot;

                if (turnUser == User.bot && !isGameOver) Invoke(nameof(MakeAIMove), Random.Range(0.7f, 1.5f));
                break;
            case GameMode.vsPlayer:
                turnUser = (turnUser == User.client) ? User.player : User.client;
                break;
            case GameMode.online:
                turnUser = (turnUser == User.client) ? User.player : User.client;
                break;
        }
        uiHandler.SetTurnText(turnUser); //Display the turn text
        ActivatePlayers(turnUser);
    }

    private void ActivatePlayers(User user)
    {
        switch (user)
        {
            case User.player:
                //Activate Team 1
                team1.SetPlayersStatus(true);
                team1.SetIndicator(true);

                //Deactivate Team 2
                team2.SetPlayersStatus(false);
                team2.SetIndicator(false);
                break;
            default: //Bots And Other Players
                //Activate Team 2
                team2.SetPlayersStatus(true);
                team2.SetIndicator(true);

                //Deactivate Team 1
                team1.SetPlayersStatus(false);
                team1.SetIndicator(false);
                break;
        }
    }

    private void ResetPlayerPieces()
    {

    }

    public void ClearBoard()
    {
        ResetPlayerPieces();
    }

    public void CheckBoardState()
    {
        
    }

    public void CheckWinState()
    {
        if (localState.Player1Scores + localState.Player2Scores >= 3)
        {
            //Game Has Been Won
            isGameOver = true;

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

    public async void GetGameState()
    {
        try
        {
            var result = await SessionHandler.GetSessionGameState(GameManager.gameSession.sessionId.ToString());
            var tempState = JsonConvert.DeserializeObject<MiniBallState>(JsonConvert.SerializeObject(result));

            ProcessState(tempState);
            Invoke(nameof(GetGameState), 5f); //Call itself again after 5 seconds
        }
        catch (System.Exception e)
        {
            NotificationDisplay.instance.DisplayMessage("Error getting the game state from the server: " + e.Message, NotificationType.error);
        }
    }

    public void ProcessState(MiniBallState state)
    {

    }
}

