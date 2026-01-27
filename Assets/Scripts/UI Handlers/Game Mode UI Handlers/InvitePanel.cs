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
        var username = usernameField.text;

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

        //TODO: Create match and get the match Id from the Server
        CreateSessionRequest request = new CreateSessionRequest
        {
            Name = GameManager.gameSession.gameName.ToString(),
            HostName = "Matthew" //Temporary placeholder until we get the actual username
        };

        var t = await SessionHandler.CreateSession(request);
        if(t != null)
        {
            //Loaclly store the session details
            GameManager.gameSession = new Wagr.Session(t.SessionId, wager, t.HostPlayerId, "-1");
        }
        Debug.Log("Session Id: " + t.SessionId.ToString());


        /*
        /Confirm if username is valid and then allow the player in
        if (CloudSaveSystem.IsNameTaken(username).Result == true)
        {
            playButton.interactable = false; //Disable the Play button to avoid multiple clicks


            //Send out the notification invite after getting the session ID
            CloudSaveSystem.SendMatchInvite(username, (int)GameManager.gameSession.gameName, wager, "0").Wait();

            //Load the actual game level and set the scene accordingly
            GameManager.instance.GoToSelectedGame();
        }
        else //Else, display a warning message that no such user exists
        {
            NotificationDisplay.instance.DisplayMessage("Error finding the specified player!", NotificationType.error);
            playButton.interactable = true;
        }
        */
    }
}
