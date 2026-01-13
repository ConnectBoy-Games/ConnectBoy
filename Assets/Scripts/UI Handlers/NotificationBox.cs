using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationBox : MonoBehaviour
{
    public Wagr.MatchInvite notification;


    [SerializeField] private TMP_Text userText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text inviteText;

    [Header("Game Icons")]
    [SerializeField] GameObject xandoImage;
    [SerializeField] GameObject fiarImage;
    [SerializeField] GameObject dotsandboxesImage;
    [SerializeField] GameObject minigolfImage;
    [SerializeField] GameObject minisoccerImage;
    [SerializeField] GameObject archeryImage;

    [Header("Background Colors")]
    [SerializeField] Image backgroundImage;
    [SerializeField] Color[] colors;

    public void SetBoxDetails(Wagr.MatchInvite invite)
    {
        notification = invite;

        //Set the title and time text
        string gameName = "";
        userText.text = notification.senderUsername;
        timeText.text = System.DateTimeOffset.FromUnixTimeSeconds(notification.timestamp).ToLocalTime().ToString("g");
    
        //Set Background Color (Randomly)
        backgroundImage.color = colors[Random.Range(0, colors.Length)];

        //Set the game image
        switch((Wagr.GameName)notification.matchType)
        {
            case Wagr.GameName.xando:
                xandoImage.SetActive(true);
                gameName = "X And O";
                break;
            case Wagr.GameName.archery:
                archeryImage.SetActive(true);
                gameName = "Archery";
                break;
            case Wagr.GameName.dotsandboxes:
                dotsandboxesImage.SetActive(true);
                gameName = "Dots And Boxes";
                break;
            case Wagr.GameName.fourinarow:
                fiarImage.SetActive(true);
                gameName = "Four In A Row";
                break;
            case Wagr.GameName.minigolf:
                minigolfImage.SetActive(true);
                gameName = "Mini Golf";
                break;
            case Wagr.GameName.minisoccer:
                minisoccerImage.SetActive(true);
                gameName = "Mini Soccer";
                break;
        }

        ///Set the message text
        inviteText.text = "Invites you to play a Game of " + gameName + " with a Wager of " + "<color = green>" + notification.wagerAmount + "</color>";
    }

    public void GoToGame()
    {
        //Set the game session
        GameManager.gameSession = new Wagr.Session(notification.matchId, notification.wagerAmount, GameManager.instance.accountManager.playerProfile.id, notification.senderId);
        
        //Load the actual game level and set the scene accordingly
        GameManager.instance.GoToSelectedGame();
    }
}
