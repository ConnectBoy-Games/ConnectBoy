using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wagr;

public class NotificationBox : MonoBehaviour
{
    public MatchInvite notification;
    public InviteDisplay inviteDisplay;

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

    public void SetBoxDetails(MatchInvite invite)
    {
        notification = invite;

        //Set the title and time text
        string gameName = "";
        userText.text = notification.senderUsername;
        //timeText.text = System.DateTimeOffset.FromUnixTimeSeconds(notification.timestamp).ToLocalTime().ToString("g");

        //Set Background Color (Randomly)
        backgroundImage.color = colors[UnityEngine.Random.Range(0, colors.Length)];

        //Set the game image
        switch ((Wagr.GameName)notification.matchType)
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
        inviteText.text = "Invites you to play a Game of " + gameName + " with a Wager of " + "<color=green>" + notification.wagerAmount + "</color>";
    }

    public void OpenPrompt()
    {
        inviteDisplay.ShowInvite();
        inviteDisplay.acceptInvite = GoToGame;
        inviteDisplay.rejectInvite = DeclineInvite;
    }

    public async void GoToGame()
    {
        LoadScreen.instance.ShowScreen();
        try
        {
            //Get the host player
            Player hostPlayer = await CloudSaveSystem.RetrieveSpecificData<Player>(notification.senderId);

            //Set the current player as the other player
            JoinSessionRequest joinRequest = new JoinSessionRequest
            {
                OtherPlayer = GameManager.instance.accountManager.playerProfile
            };

            //Try to join the game session
            var joined = await SessionHandler.JoinSession(notification.matchId, joinRequest);

            if(joined.Status == false) //Failed to join
            {
                NotificationDisplay.instance.DisplayMessage("Error joining game");
                return;
            }

            //Set the local game session reference to match
            GameManager.gameSession = new Session(Guid.Parse(notification.matchId), (GameName)notification.matchType, notification.wagerAmount, hostPlayer, GameManager.instance.accountManager.playerProfile, GameRole.friend);

            //Load the actual game level and set the scene accordingly
            GameManager.instance.GoToSelectedGame();
        }
        catch (System.Exception ex)
        {
            NotificationDisplay.instance.DisplayMessage("Error joining game: " + ex.Message, NotificationType.error);
        }
    }

    public void DeclineInvite()
    {
        Destroy(gameObject);
    }
}
