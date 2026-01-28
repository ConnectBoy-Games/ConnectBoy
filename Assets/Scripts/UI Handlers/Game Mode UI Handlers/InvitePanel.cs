using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvitePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField wagerField;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private Button playButton;

    private void OnEnable()
    {
        wagerField.text = "";
        usernameField.text = "";
        playButton.interactable = true;
    }

    public async void InvitePlayer()
    {
        //Check the Wager value
        var wager = int.Parse(wagerField.text);
        var username = usernameField.text; //Username of the player to invite

        if (wager < 100)
        {
            NotificationDisplay.instance.DisplayMessage("The Wager amount is too low!", NotificationType.info);
            return;
        }
        else if (wager > 20000)
        {
            NotificationDisplay.instance.DisplayMessage("The Wager amount is too high!", NotificationType.info);
            return;
        }

        playButton.interactable = false; //Disable the Play button to avoid multiple clicks

        //TODO: Create match and get the match Id from the Server
        CreateSessionRequest request = new CreateSessionRequest
        {
            Name = "Game",
            GameName = GameManager.gameSession.gameName,
            Wager = wager,
            HostPlayer = GameManager.instance.accountManager.playerProfile,
        };

        var session = await SessionHandler.CreateSession(request);
        if(session != null)
        {
            //Locally store the session details
            GameManager.gameSession = new Wagr.Session(session.SessionId, wager, GameManager.instance.accountManager.playerProfile, null);
        }
        
        if (CloudSaveSystem.IsNameTaken(username).Result == true)
        {
            //Send out the notification invite after getting the session ID
            await CloudSaveSystem.SendMatchInvite(username, (int)GameManager.gameSession.gameName, wager, session.SessionId.ToString());

            //Load the actual game level and set the scene accordingly
            GameManager.instance.GoToSelectedGame();
        }
        else //Else, display a warning message that no such user exists
        {
            NotificationDisplay.instance.DisplayMessage("Error finding the specified player!", NotificationType.error);
            playButton.interactable = true;
        }

        playButton.interactable = true;
    }
}
