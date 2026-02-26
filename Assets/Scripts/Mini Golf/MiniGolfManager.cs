using System.Collections;
using System.Collections.Generic;
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

    [Header("Game References")]
    [SerializeField] private MiniGolfMap golfMap;
    [SerializeField] private Transform ballObject;

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
        uiHandler.SetTurnText(turnUser); //Display the turn text
    }

    public async void OnLocalPlayerShoot(Vector2 shootForce)
    {
        if (isGameOver)
        {
            Handheld.Vibrate();
            return;
        }

        if (GameManager.gameMode == GameMode.vsBot) //&& turnUser == User.client)
        {
            // 1. Run physics locally so we see it happen instantly
            var trajectory = CustomGolfPhysics.SimulateShot(ballObject.position, shootForce, golfMap.currentLevelWalls);

            // 2. Animate the ball (Coroutine to move along 'trajectory' list)
            StartCoroutine(AnimateBall(trajectory));

            localState.Player1Scores++; //Increase the move counter
            //SwitchTurns();
        }
        else if (GameManager.gameMode == GameMode.vsPlayer)
        {

        }
        else if (GameManager.gameMode == GameMode.online)
        {
            MiniGolfMove move = new MiniGolfMove
            {
                X = shootForce.x,
                Y = shootForce.y
            };

            //Send move to server
            var result = await SessionHandler.MakeMove(GameManager.gameSession.sessionId.ToString(), move);
            var tempState = JsonConvert.DeserializeObject<MiniGolfState>(JsonConvert.SerializeObject(result.State));

            ProcessState(tempState); //Process the game state
        }
        else
        {
            Handheld.Vibrate();
        }
    }

    private void MakeAIMove()
    {
        if (isGameOver) return;

        Vector2 botForce = bot.ThinkMove(ballObject.position);

        var trajectory = CustomGolfPhysics.SimulateShot(ballObject.position, botForce, golfMap.currentLevelWalls);
        StartCoroutine(AnimateBall(trajectory));

        localState.Player2Scores++;
    }

    // ... helper Coroutine AnimateBall() code here ...
    // Smoother version using Vector3.MoveTowards
    private IEnumerator AnimateBall(List<Vector2> trajectory)
    {
        foreach (Vector2 targetStep in trajectory)
        {
            // While the ball is not yet at the next physics step...
            while (Vector3.Distance(ballObject.transform.position, targetStep) > 0.01f)
            {
                // Move towards it smoothly over time
                ballObject.transform.position = Vector3.MoveTowards(
                    ballObject.transform.position,
                    targetStep,
                    10f * Time.deltaTime // Adjust speed as needed
                );

                // Wait for the next frame
                yield return null;
            }
        }

        // Update current ball position after animation ends
        //currentBallPos = ballObject.transform.position;

        // Check if the game is over (e.g., ball in hole) before switching turns
        CheckBoardState();
        if (!isGameOver) SwitchTurns();
    }

    public void ClearBoard()
    {
        isGameOver = false;
        //currentBallPos = ballPrefab.transform.position;
    }

    public void CheckBoardState()
    {
        float distToHole = Vector2.Distance(ballObject.position, (Vector2)golfMap.holeTransform.position);
        if (distToHole < 0.3f) // Threshold for sinking the ball
        {
            isGameOver = true;
            uiHandler.DisplayWinScreen(turnUser == User.client ? "You Won!" : "Bot Won!");
        }
    }

    public int CheckWinState(string piece)
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
        var trajectory = CustomGolfPhysics.SimulateShot(ballObject.position, opponentForce, golfMap.currentLevelWalls);

        // 3. Play the animation
        StartCoroutine(AnimateBall(trajectory));
    }
}
