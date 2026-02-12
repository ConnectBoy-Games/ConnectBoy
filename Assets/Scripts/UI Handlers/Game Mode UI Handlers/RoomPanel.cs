using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] private List<Button> roomListing;
    [SerializeField] private Button playButton;

    private int wagerRoom = -1;

    void OnEnable()
    {
        wagerRoom = -1;
        playButton.interactable = false;
        foreach (var item in roomListing)
        {
            item.onClick.AddListener(() =>
            {
                item.GetComponent<Image>().CrossFadeColor(Color.cyan, 1f, true, true);
            });
        }
    }

    public void SetWager(int amount)
    {
        wagerRoom = amount;
        playButton.interactable = true;
    }

    public async void CreateRoom()
    {
        int wager = wagerRoom;

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
        else
        {
            LoadScreen.instance.HideScreen();
            playButton.interactable = true;
            NotificationDisplay.instance.DisplayMessage("Error creating a game session!", NotificationType.error);
        }
    }

    public void GoToGame()
    {
        GameManager.instance.GoToSelectedGame(); //Load the actual game level and set the scene accordingly
    }
}
