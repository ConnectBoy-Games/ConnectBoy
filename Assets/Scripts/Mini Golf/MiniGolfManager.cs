using Newtonsoft.Json;
using UnityEngine;

public class MiniGolfManager : MonoBehaviour, IGameManager
{
    private MiniGolfState localState = new();
    private MiniGolfBot bot;
    private User turnUser;

    private bool isGameOver = false;

    [Header("UI Handling")]
    [SerializeField] MiniGolfUIHandler uiHandler;

    [Header("Game Handling")]
    [SerializeField] private Transform ball;
    [SerializeField] private MiniGolfMap golfMap;

    async void OnEnable() //The entry point of the Game Manager
    {
        ClearBoard();

        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (User)Random.Range(1, 3); //Set who has the turn
                uiHandler.SetTurnText(turnUser);

                bot = new MiniGolfBot(GameManager.botDifficulty, golfMap); //Set the bot difficulty
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

                if (turnUser == User.bot && !isGameOver) Invoke(nameof(MakeAIMove), Random.Range(0.7f, 1.5f));
                break;
            case GameMode.vsPlayer:
                turnUser = (turnUser == User.client) ? User.player : User.client;
                break;
            case GameMode.online:
                turnUser = (turnUser == User.client) ? User.player : User.client;
                break;
        }

        switch (turnUser)
        {
            case User.bot:
                ball.GetComponent<MiniGolfBall>().isPlayable = false;
                break;
            case User.client:
                ball.GetComponent<MiniGolfBall>().isPlayable = true;
                break;
            case User.player:
                ball.GetComponent<MiniGolfBall>().isPlayable = true;
                break;
        }

        ball.GetComponent<MiniGolfBall>().locked = false;
        uiHandler.SetTurnText(turnUser); //Display the turn text
    }

    public void MakeMove()
    {
        switch (turnUser)
        {
            case User.client:
                localState.Player1Scores++;
                break;
            case User.bot:
                localState.Player2Scores++;
                break;
            case User.player:
                localState.Player2Scores++;
                break;
        }

        /* If we are online, we need to send the move to the server
        MiniGolfMove move = new MiniGolfMove
        {
            X = shootForce.x,
            Y = shootForce.y
        };

        //Send move to server
        var result = await SessionHandler.MakeMove(GameManager.gameSession.sessionId.ToString(), move);
        var tempState = JsonConvert.DeserializeObject<MiniGolfState>(JsonConvert.SerializeObject(result.State));
        ProcessState(tempState); //Process the game state
        */

        //Update the score panel with the new score values
        ScorePanel.instance.UpdateScore(localState.Player1Scores, localState.Player2Scores);
    }

    // Called by the ball after it stops moving
    public void BallMoved()
    {
        if (golfMap.CheckBall(ball.position)) //Checks if the ball is in the hole
        {
            SwitchTurns();
            ClearBoard(); //Reset the ball position for the next round
            CheckWinState();
        }
    }

    private void MakeAIMove()
    {
        if (isGameOver) return;

        Vector2 botForce = bot.ThinkMove(ball.position);
        ball.GetComponent<MiniGolfBall>().MakeMove(botForce);
    }

    public void ClearBoard()
    {
        isGameOver = false;
        //currentBallPos = ballPrefab.transform.position;
    }

    public int CheckWinState()
    {
        throw new System.NotImplementedException();
    }

    public async void GetGameState()
    {
        try
        {
            var result = await SessionHandler.GetSessionGameState(GameManager.gameSession.sessionId.ToString());
            var tempState = JsonConvert.DeserializeObject<MiniGolfState>(JsonConvert.SerializeObject(result));

            ProcessState(tempState);
            Invoke(nameof(GetGameState), 5f); //Call itself again after 5 seconds
        }
        catch (System.Exception e)
        {
            NotificationDisplay.instance.DisplayMessage("Error getting the game state from the server: " + e.Message, NotificationType.error);
        }
    }

    void ProcessState(MiniGolfState state)
    {
        string json = "";

        // 1. Deserialize
        MiniGolfMove move = JsonConvert.DeserializeObject<MiniGolfMove>(json);
        Vector2 opponentForce = new Vector2(move.X, move.Y);

        // 2. Run the EXACT same physics engine
        var trajectory = CustomGolfPhysics.SimulateShot(ball.position, opponentForce, golfMap.walls);
    }
}
