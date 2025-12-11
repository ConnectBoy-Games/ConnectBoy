using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationBox : MonoBehaviour
{
    public Wagr.NotificationObject notification;

    public Image gameImage;
    [SerializeField] private TMP_Text userText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text inviteText;

    public void SetBoxDetails()
    {
        userText.text = notification.userName;
        timeText.text = notification.timeText;
        inviteText.text = "Invites you to play a Game of " + notification.gameName.ToString() + " with a Wagr of " + notification.wager.ToString();
    }

    public void GoToGame()
    {

    }
}
