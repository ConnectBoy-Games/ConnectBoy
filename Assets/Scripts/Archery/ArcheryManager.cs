using UnityEngine;

public class ArcheryManager : MonoBehaviour, IGameManager
{
    private ArcheryState localState;
    private ArcheryBot bot;
    private User turnUser;

    [Header("UI Handling")]
    [SerializeField] ArcheryUIHandler uiHandler;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchTurns()
    {
        switch (GameManager.gameMode)
        {
            case GameMode.vsBot:
                turnUser = (turnUser == User.bot) ? User.client : User.bot;
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

    public void ClearBoard()
    {
        throw new System.NotImplementedException();
    }

    public void CheckBoardState()
    {
        throw new System.NotImplementedException();
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
