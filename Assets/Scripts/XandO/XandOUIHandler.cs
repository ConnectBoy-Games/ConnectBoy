using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XandOUIHandler : MonoBehaviour, IGameUIHandler
{
    [SerializeField] private XandOManager manager;

    [SerializeField] TMP_Text turnText;
    [SerializeField] GameObject chatPanel;
    [SerializeField] GameObject forfeitPanel;

    [Header("Win Or Lose Panels")]
    [SerializeField] GameObject endPanel;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] GameObject defeatPanel;
    [SerializeField] TMP_Text victoryText;
    [SerializeField] TMP_Text defeatText;

    [SerializeField] GameObject chatButton;

    void Start()
    {
        if (GameManager.gameSession.gameMode == GameMode.vsBot) //Disable the Chat Button if we are playing with a bot
        {
            chatButton.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            GoBack();
        }
    }

    public void GoBack()
    {
        if (endPanel.activeInHierarchy || defeatPanel.activeInHierarchy)
        {
            //GoToHome();
        }
        else if (chatPanel.activeInHierarchy)
        {
            chatPanel.SetActive(false);
        }
        else
        {
            forfeitPanel.SetActive(true);
        }
    }

    public void GoToHome()
    {
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
    }

    public void SetTurnText(User turnUser, string text = null)
    {
        if (text == null)
        {
            if (GameManager.instance.accountManager.loginState == LoginState.loggedIn)
            {
                switch (turnUser)
                {
                    case User.player:
                        turnText.text = GameManager.gameSession.gameRole == GameRole.host ? manager.det.OtherPlayer.Name + "'s turn" : manager.det.HostPlayer.Name + "'s turn";
                        break;
                    case User.client:
                        turnText.text = GameManager.instance.accountManager.playerProfile.Name + "'s turn";
                        break;
                }
            }
            else
            {
                switch (turnUser)
                {
                    case User.bot:
                        turnText.text = "Bot's Move";
                        break;
                    case User.client:
                        turnText.text = "Your Move";
                        break;
                }
            }
        }
        else
        {
            turnText.text = text;
        }
    }

    public void DisplayWinScreen(string name, int wager = -1)
    {
        endPanel.SetActive(true);
        victoryPanel.SetActive(true);
        victoryText.text = name + wager.ToString(); ;
    }

    public void DisplayDefeatScreen(string name, int wager = -1)
    {
        endPanel.SetActive(true);
        defeatPanel.SetActive(true);
        defeatText.text = name + wager.ToString(); ;

    }
}
