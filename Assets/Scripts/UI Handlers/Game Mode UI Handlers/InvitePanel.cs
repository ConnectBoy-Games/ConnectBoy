using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvitePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField wagerField;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private Button playButton;

    private void Update()
    {
        //TODO: Add dynamic keyboard movement
        //wagerField.onTouchScreenKeyboardStatusChanged
    }

    private void OnEnable()
    {
        wagerField.text = "";
        usernameField.text = "";
        playButton.interactable = true;
    }

    public async void InvitePlayer()
    {
        int wager = int.Parse(wagerField.text);
        string username = usernameField.text; //Username of the player to invite

        //Check the Wager value
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

        if(username == GameManager.instance.accountManager.playerProfile.Name)
        {
            NotificationDisplay.instance.DisplayMessage("You can't send an invite to yourself", NotificationType.error);
            playButton.interactable = true;
            return;
        }

        //Check if it is a valid user
        LoadScreen.instance.ShowScreen("Checking Player Details!");
        bool isNameTaken = await CloudSaveSystem.IsNameTaken(username);

        if (isNameTaken)
        {
            LoadScreen.instance.ShowScreen("Creating Game Session!");
            CreateSessionRequest request = new CreateSessionRequest
            {
                Name = "Game",
                GameName = GameManager.gameSession.gameName,
                Wager = wager,
                HostPlayer = GameManager.instance.accountManager.playerProfile,
            };
            CreateSessionResponse session = await SessionHandler.CreateSession(request);
            
            if (session != null)
            {
                //Locally store the session details
                GameManager.gameSession = new Wagr.Session(session.SessionId, request.GameName, wager, request.HostPlayer, null, GameRole.host);
            }

            //Send out the notification invite after getting the session ID
            LoadScreen.instance.ShowScreen("Sending Out Game Notification!");
            await CloudSaveSystem.SendMatchInvite(username, (int)GameManager.gameSession.gameName, wager, session.SessionId.ToString());
            GoToGame();
        }
        else //Else, display a warning message that no such user exists
        {
            NotificationDisplay.instance.DisplayMessage("Error finding the specified player!", NotificationType.error);
            LoadScreen.instance.HideScreen();
            playButton.interactable = true;
        }

        LoadScreen.instance.HideScreen();
        playButton.interactable = true;
    }

    private void GoToGame()
    {
        GameManager.instance.GoToSelectedGame(); //Load the actual game level and set the scene accordingly
    }
}
