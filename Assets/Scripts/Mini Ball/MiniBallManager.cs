using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class MiniBallManager : MonoBehaviour, IGameManager
{
    private MiniBallState localState = new();
    private MiniBallBot bot;
    private User turnUser;

    public bool isGameOver = false;
    public bool isPaused = false;

    [Header("UI Handling")]
    [SerializeField] private MiniBallUIHandler uiHandler;

    [Header("Game Handling")]
    [SerializeField] Transform ball;
    [SerializeField] List<Collider> walls; //List of the bounding walls in the game ("Not including the posts'")
    [SerializeField] TeamManager team1; //The parent object that holds the player 1 pieces
    [SerializeField] TeamManager team2; //The parent object that holds the player 2 pieces

    async void OnEnable() //The entry point of the Game Manager
    {
        isGameOver = false;
        team1.manager = this;
        team2.manager = this;
        ScorePanel.instance.UpdateScore(0, 0);
        ClearBoard();

        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (User)Random.Range(1, 3); //Set who has the turn
                uiHandler.SetTurnText(turnUser);

                bot = new MiniBallBot(GameManager.botDifficulty); //Set the bot difficulty
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

        ActivatePlayers(turnUser);
    }

    public async void MadeMove() // Called by the player piece after making a move
    {
        // Delay for 4 secs to allow the players finish moving and the ball to settle down before switching turns and recording the move
        await System.Threading.Tasks.Task.Delay(4000);

        team1.CheckPlayers(); //Check if any of the players in team 1 are stuck in the post and move them out
        team2.CheckPlayers(); //Check if any of the players in team 2 are stuck in the post and move them out

        // Continue normal processing
        SwitchTurns();

        // TODO: For online mode, send the recorded frames over the network
    }

    // Called by the ball after it stops moving
    public void BallMoved()
    {
        team1.CheckPlayers(); //Check if any of the players in team 1 are stuck in the post and move them out
        team2.CheckPlayers(); //Check if any of the players in team 2 are stuck in the post and move them out

        if (team1.CheckGoal(ball))
        {
            localState.Player1Scores++;
            ClearBoard();
        }
        else if (team2.CheckGoal(ball))
        {
            localState.Player2Scores++;
            ClearBoard();
        }

        //Update the score panel with the new score values
        ScorePanel.instance.UpdateScore(localState.Player1Scores, localState.Player2Scores);
        CheckWinState();
    }

    public void MakeAIMove()
    {
        var move = bot.MakeMove(team1, team2, ball); //Let the AI think the move

        foreach (Transform player in team2.transform)
        {
            if (player.GetComponent<PlayerPiece>().piece == move.PieceId)
            {
                player.GetComponent<PlayerPiece>().MakeMove(new Vector3(move.forceX, 0, move.forceZ) / 3);
                break;
            }
        }
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
        PlayerPiece.locked = false;
        team1.reportedMove = false;
        team2.reportedMove = false;

        switch (user)
        {
            case User.client:
                //Activate Team 1
                team1.SetPlayersStatus(true);
                team1.SetIndicator(true);

                //Deactivate Team 2
                team2.SetPlayersStatus(false);
                team2.SetIndicator(false);
                break;

            case User.bot:
                //Activate Team 2
                team2.SetIndicator(true);

                //Deactivate Team 1
                team1.SetPlayersStatus(false);
                team1.SetIndicator(false);
                break;

            case User.player: //Bots And Other Players
                //Activate Team 2
                team2.SetPlayersStatus(true);
                team2.SetIndicator(true);

                //Deactivate Team 1
                team1.SetPlayersStatus(false);
                team1.SetIndicator(false);
                break;
        }
    }

    public void ClearBoard()
    {
        //Set the same formation for both teams
        int formationIndex = Random.Range(0, team1.formations.Count);
        team1.SetFormation(formationIndex);
        team2.SetFormation(formationIndex);
        ball.position = Vector3.zero;
    }

    public void CheckWinState()
    {
        if (localState.Player1Scores >= 3 || localState.Player2Scores >= 3) //Game Has Been Won
        {
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

