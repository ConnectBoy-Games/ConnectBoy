using UnityEngine;

public class UIFlowHandler : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject profilePanel;
    [SerializeField] private GameObject gameDetailPanel;
    [SerializeField] private GameObject gamesPanel;
    [SerializeField] private GameObject loginPanel;

    [SerializeField] private GameObject notificationDisplay;
    [SerializeField] private GameObject loadScreenDisplay;
    [SerializeField] private GameObject inviteDisplay;

    void Awake()
    {
        notificationDisplay.SetActive(true);
        loadScreenDisplay.SetActive(true);
        inviteDisplay.SetActive(true);

        /*
        if(GameManager.instance.accountManager.loginState == LoginState.unsigned)
        {
            menuPanel.SetActive(false);
            gamesPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else
        {
            GotoMainMenu();
        }
        */
    }

    void Start()
    {
        loginPanel.GetComponent<LoginPanel>().backAction = GotoMainMenu;
        settingsPanel.GetComponent<SettingsPanel>().backAction = GotoMainMenu;
        profilePanel.GetComponent<ProfilePanel>().backAction = GotoMainMenu;
        gameDetailPanel.GetComponent<GameDetailPanel>().backAction = GotoMainMenu;
        notificationPanel.GetComponent<NotificationPanel>().backAction = GotoMainMenu;
    }

    public void GoToNotification()
    {
        notificationPanel.SetActive(true);
    }

    public void GotoSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void GotoProfile()
    {
        if(GameManager.instance.accountManager.loginState != LoginState.loggedIn)
        {
            GameManager.instance.GetComponent<AudioManager>().PlayClickSound();
            DisableAll();
            loginPanel.SetActive(true);
            return;
        }

        profilePanel.SetActive(true);
    }

    public void GotoGameDetail(int game)
    {
        gameDetailPanel.SetActive(true);
        gameDetailPanel.GetComponent<GameDetailPanel>().SelectGame((Wagr.GameName)game);
        GameManager.gameSession = new((Wagr.GameName)game); //Set the details of the game manager
    }

    public void GotoMainMenu()
    {
        DisableAll();
        menuPanel.SetActive(true);
        gamesPanel.SetActive(true);
    }

    public void DisableAll()
    {
        menuPanel.SetActive(false);
        profilePanel.SetActive(false);
        gamesPanel.SetActive(false);
        settingsPanel.SetActive(false);
        gameDetailPanel.SetActive(false);
        loginPanel.SetActive(false);
        notificationPanel.SetActive(false);
    }
}
