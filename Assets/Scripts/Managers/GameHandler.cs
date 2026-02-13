using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [SerializeField] GameObject goHomeButton;
    [SerializeField] GameObject manager; //The object that holds the manager for the game

    private int runCount = 0;

    public async void CheckStatus()
    {
        runCount++;

        if (GameManager.gameSession.gameRole == GameRole.host) //Only do this check if you are the host and are waiting for a player to join the game
        {
            SessionDetails det = await SessionHandler.CheckSessionStatus(GameManager.gameSession.sessionId.ToString());
            if (det.OtherPlayer != null) //Second player has joined the game
            {
                GameManager.gameSession.other = det.OtherPlayer; //Update the local game session
                ScorePanel.instance.SetUsernames(det.HostPlayer.Name, det.OtherPlayer.Name); //Set the usernames in the score panel
                GameManager.instance.GetComponent<AudioManager>().PlayAcceptSound();

                manager.SetActive(true);
                gameObject.SetActive(false);
                CancelInvoke(nameof(CheckStatus));
            }
        }
        else //Just go straight to the game, no need to wait for 2nd player
        {
            ScorePanel.instance.SetUsernames(GameManager.gameSession.client.Name, GameManager.gameSession.other.Name); //Set the usernames in the score panel
            GameManager.instance.GetComponent<AudioManager>().PlayAcceptSound();

            manager.SetActive(true);
            gameObject.SetActive(false);
            CancelInvoke(nameof(CheckStatus));
        }

        if (runCount == 10)
        {
            GameManager.instance.GetComponent<AudioManager>().PlayErrorSound();
            goHomeButton.SetActive(true);
        }
    }

    public async void ExitSession()
    {
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
        /*
        SessionDetails det = await SessionHandler.CheckSessionStatus(GameManager.gameSession.sessionId.ToString());

        if(det.OtherPlayer == null)
        {
            await SessionHandler.DestroySession(GameManager.gameSession.sessionId.ToString());
            //TODO: Check if the session was destroyed and the go back to main menu 
        }
        */
    }

    void Start()
    {
        if (GameManager.gameMode == GameMode.online)
        {
            runCount = 0;
            InvokeRepeating(nameof(CheckStatus), 1f, 5f); //Keep checking the server if the other player has connected
            LoadScreen.instance.ShowScreen("Waiting For Other Player To Join! \n ...");
        }
        else
        {
            manager.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
