using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XandOUIHandler : MonoBehaviour
{
    [SerializeField] private XandOManager manager;

    [SerializeField] TMP_Text turnText;
    [SerializeField] GameObject chatPanel;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] GameObject defeatPanel;
    [SerializeField] GameObject forfeitPanel;

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
        if (victoryPanel.activeInHierarchy || defeatPanel.activeInHierarchy)
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
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void SetTurnText(User turnUser, string text = null)
    {
        if (text == null)
        {
            switch (turnUser)
            {
                case User.bot:
                    turnText.text = "Bot's Move";
                    break;
                case User.host:
                    turnText.text = "Your Move";
                    break;
                case User.player:
                    turnText.text = "Player's Move";
                    break;
            }
        }
        else
        {
            turnText.text = text;
        }
    }

    public void DisplayWinScreen(string name, int wager = -1)
    {
        victoryPanel.SetActive(true);
    }
}
