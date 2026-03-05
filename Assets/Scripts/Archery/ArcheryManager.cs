using Newtonsoft.Json;
using UnityEngine;

public class ArcheryManager : MonoBehaviour, IGameManager
{
    private ArcheryState localState = new();
    private ArcheryBot bot;
    private User turnUser;

    [Header("UI Handling")]
    [SerializeField] ArcheryUIHandler uiHandler;

    [Header("Bot Settings")]
    // The centre of the archery target board in world space
    [SerializeField] Transform targetCenter;
    // Outer radius of the scored ring area on the board
    [SerializeField] float targetRadius = 1.5f;
    // Arrow spawn point (matches leftSpawn / rightSpawn in ArcheryControl)
    [SerializeField] Transform arrowSpawnPoint;
    // Prefab used to fire the bot's arrow (same arrowPrefab as ArcheryControl)
    [SerializeField] GameObject arrowPrefab;

    async void OnEnable()
    {
        ClearBoard();

        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (User)Random.Range(1, 3); //Set who has the turn
                uiHandler.SetTurnText(turnUser);

                bot = new ArcheryBot(GameManager.botDifficulty); //Set the bot difficulty
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

                if(turnUser == User.bot) Invoke(nameof(MakeAIMove), 1f); //Make an AI move if it has the turn
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

    public void MakeAIMove()
    {
        if (bot == null || targetCenter == null || arrowSpawnPoint == null || arrowPrefab == null)
        {
            SwitchTurns();
            return;
        }

        // Ask the bot where to aim
        Vector3 aimPoint = bot.ThinkMove(
            targetCenter.position,
            targetRadius,
            arrowSpawnPoint.position
        );

        // Spawn the arrow from the bot's spawn point and fire it
        var arrowGO = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        if (arrowGO.TryGetComponent(out Arrow arrow))
        {
            ArcheryControl.ArrowState state = ArcheryControl.ArrowState.Released;
            arrow.SetTarget(aimPoint, ref state);
        }

        SwitchTurns();
    }

    public void ClearBoard()
    {
        //throw new System.NotImplementedException();
    }

    public void CheckBoardState()
    {
        throw new System.NotImplementedException();
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
            var tempState = JsonConvert.DeserializeObject<ArcheryState>(JsonConvert.SerializeObject(result));

            ProcessState(tempState);
            Invoke(nameof(GetGameState), 5f); //Call itself again after 5 seconds
        }
        catch (System.Exception e)
        {
            NotificationDisplay.instance.DisplayMessage("Error getting the game state from the server: " + e.Message, NotificationType.error);
        }
    }

    public void ProcessState(ArcheryState state)
    {

    }
}
