using TMPro;
using UnityEngine;

public class DotsAndBoxesUIHandler : MonoBehaviour
{
    [SerializeField] DotsAndBoxesManager manager;

    [Header("UI Elements")]
    [SerializeField] TMP_Text turnText;
    [SerializeField] TMP_Text botScore;
    [SerializeField] TMP_Text playerScore;

    void UpdateUI()
    {
        
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
}
