using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FourInOneUIHandler : MonoBehaviour
{
    [SerializeField] TMP_Text turnText;
    [SerializeField] GameObject chatPanel;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] GameObject defeatPanel;
    [SerializeField] GameObject forfeitPanel;

    [Header("Game Updates")]
    [SerializeField] TMP_Text playerScore;
    [SerializeField] TMP_Text friendScore;

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
            GoToHome();
        }
        else if (chatPanel.activeInHierarchy)
        {
            DisableChatPanel();
        }
        else
        {
            forfeitPanel.SetActive(true);
        }
    }

    private void GoToHome()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void EnableChatPanel()
    {
        chatPanel.SetActive(true);
    }

    public void DisableChatPanel()
    {
        chatPanel.SetActive(false);
    }

    #region UI Updates
    public void SetTurnText(User turnUser, string text = null)
    {
        if (text == null)
        {
            switch (turnUser)
            {
                case User.bot:
                    turnText.text = "Bot's Move";
                    break;
                case User.client:
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
    #endregion

    #region Server Connecting Functions

    #endregion
}
