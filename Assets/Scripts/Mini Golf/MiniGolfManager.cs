using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class MiniGolfManager : MonoBehaviour, IGameManager
{
    private MiniGolfGameState localState;
    private User turnUser; //Who has the tun
    private MiniGolfBot bot; //The golf bot
    private bool isGameover;

    [Header("UI Handling")]
    [SerializeField] MiniGolfUIHandler uiHandler;

    [Header("Game References")]
    public GameObject ballPrefab;
    [SerializeField] private Transform holeTransform; 
    private Vector2 currentBallPos;
    private List<WallData> currentLevelWalls = new List<WallData>();

    private void Start()
    {
        if (GameManager.gameMode == GameMode.vsBot)
        {
            bot = new MiniGolfBot(GameManager.botDifficulty, holeTransform.position, currentLevelWalls);
        }
        currentBallPos = ballPrefab.transform.position;
    }

    public void SwitchTurns()
    {
        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (turnUser == User.bot) ? User.client : User.bot;
                if (turnUser == User.bot && !isGameover)
                {
                    Invoke(nameof(MakeAIMove), Random.Range(1f, 2f));
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

    private void MakeAIMove()
    {
        if (isGameover) return;
        
        Vector2 botForce = bot.ThinkMove(currentBallPos);
        OnAIMove(botForce);
    }

    private void OnAIMove(Vector2 shootForce)
    {
        var trajectory = CustomGolfPhysics.SimulateShot(currentBallPos, shootForce, currentLevelWalls);
        StartCoroutine(AnimateBall(trajectory));
    }

    // Called when YOU shoot
    public void OnLocalPlayerShoot(Vector2 shootForce)
    {
        // 1. Run physics locally so we see it happen instantly
        var trajectory = CustomGolfPhysics.SimulateShot(currentBallPos, shootForce, currentLevelWalls);

        // 2. Animate the ball (Coroutine to move along 'trajectory' list)
        StartCoroutine(AnimateBall(trajectory));

        // 3. Create the packet to send over HTTP
        MiniGolfMove move = new MiniGolfMove
        {
            X = shootForce.x,
            Y = shootForce.y
        };

        // 4. Serialize and Send (JSON)
        string json = JsonConvert.SerializeObject(move);
        Debug.Log("Sending to server: " + json);
    }

    // Called when HTTP request arrives with Opponent's move
    public void OnOpponentMoveReceived(string json)
    {
        // 1. Deserialize
        MiniGolfMove move = JsonConvert.DeserializeObject<MiniGolfMove>(json);
        Vector2 opponentForce = new Vector2(move.X, move.Y);

        // 2. Run the EXACT same physics engine
        var trajectory = CustomGolfPhysics.SimulateShot(currentBallPos, opponentForce, currentLevelWalls);

        // 3. Play the animation
        StartCoroutine(AnimateBall(trajectory));
    }

    // ... helper Coroutine AnimateBall() code here ...
    // Smoother version using Vector3.MoveTowards
    private IEnumerator AnimateBall(List<Vector2> trajectory)
    {
        foreach (Vector2 targetStep in trajectory)
        {
            // While the ball is not yet at the next physics step...
            while (Vector3.Distance(ballPrefab.transform.position, targetStep) > 0.01f)
            {
                // Move towards it smoothly over time
                ballPrefab.transform.position = Vector3.MoveTowards(
                    ballPrefab.transform.position,
                    targetStep,
                    10f * Time.deltaTime // Adjust speed as needed
                );

                // Wait for the next frame
                yield return null;
            }
        }

        // Update current ball position after animation ends
        currentBallPos = ballPrefab.transform.position;
        
        // Check if the game is over (e.g., ball in hole) before switching turns
        CheckBoardState(); 
        if (!isGameover) SwitchTurns();
    }

    public void ClearBoard()
    {
        throw new System.NotImplementedException();
    }

    public void CheckBoardState()
    {
        float distToHole = Vector2.Distance(currentBallPos, (Vector2)holeTransform.position);
        if (distToHole < 0.3f) // Threshold for sinking the ball
        {
            isGameover = true;
            uiHandler.DisplayWinScreen(turnUser == User.client ? "You Won!" : "Bot Won!");
        }
    }

    public int CheckWinState(string piece)
    {
        throw new System.NotImplementedException();
    }

    public void GetGameState()
    {
        throw new System.NotImplementedException();
    }
}
