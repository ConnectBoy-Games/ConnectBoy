using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] GameObject goHomeButton;
    private int runCount = 0;

    public async void CheckStatus()
    {
        runCount++;

        SessionDetails det = await SessionHandler.CheckSessionStatus(GameManager.gameSession.sessionId.ToString());

        if(det.OtherPlayer != null) //Second player has joined the game
        {
            if(GameManager.gameSession.gameRole == GameRole.host)
            {
                ScorePanel.instance.SetUsernames(det.HostPlayer.Name, det.OtherPlayer.Name);
            }
            else
            {
                ScorePanel.instance.SetUsernames(det.OtherPlayer.Name, det.HostPlayer.Name);
            }

            gameObject.SetActive(false);
        }

        if(runCount >= 10)
        {
            goHomeButton.SetActive(true);
        }
    }

    public async void ExitSession()
    {
        SessionDetails det = await SessionHandler.CheckSessionStatus(GameManager.gameSession.sessionId.ToString());

        if(det.OtherPlayer == null)
        {
            await SessionHandler.DestroySession(GameManager.gameSession.sessionId.ToString());
            //TODO: Check if the session was destroyed and the go back to main menu 
        }
    }

    void Start()
    {
        if(GameManager.gameSession.gameMode == GameMode.vsBot)
        {
            gameObject.SetActive(false);
        }
        else
        {
            InvokeRepeating(nameof(CheckStatus), 1f, 5f); //Keep checking the server if the other player has connected
        }
    }
}
