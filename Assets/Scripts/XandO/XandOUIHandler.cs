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
        //Disable the Chat Button if we are playing with a bot
        if (GameManager.gameMode == GameMode.vsBot) chatButton.SetActive(false);
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
        if (text != null)
        {
            turnText.text = text;
        }
        else
        {
            if (GameManager.gameMode == GameMode.online)
            {
                turnText.text = (turnUser == User.player) ? GameManager.gameSession.other.Name + "'s Turn" : GameManager.gameSession.client.Name + "'s Turn";
            }
            else if (GameManager.gameMode == GameMode.vsPlayer)
            {
                turnText.text = (turnUser == User.player) ? "Player 2's Turn" : "Player 1's  Turn";
            }
            else //Bot mode
            {
                turnText.text = (turnUser == User.client) ? "Your Turn" : "Bot's  Turn";
            }
        }
    }

    public void DisplayWinScreen(string text = "", int wager = -1)
    {
        endPanel.SetActive(true);
        victoryPanel.SetActive(true);

        if (GameManager.gameMode == GameMode.online)
        {
            victoryText.text = "You have won \n" + GameManager.gameSession.wager.ToString();
        }
        else if (GameManager.gameMode == GameMode.vsPlayer)
        {
            victoryText.text = text;
        }
        else
        {
            victoryText.text = "You have won!";
        }
    }

    public void DisplayDefeatScreen(string text = "", int wager = -1)
    {
        endPanel.SetActive(true);
        defeatPanel.SetActive(true);
        //defeatText.text = name + wager.ToString(); ;

        if (GameManager.gameMode == GameMode.online)
        {
            victoryText.text = "You have lost \n" + GameManager.gameSession.wager.ToString();
        }
        else if (GameManager.gameMode == GameMode.vsPlayer)
        {
            victoryText.text = text;
        }
        else
        {
            victoryText.text = "You lost!";
        }
    }
}
