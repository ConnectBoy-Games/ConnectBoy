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

    void Awake()
    {
        /*
        if(GameManager.instance.accountManager.loginState == LoginState.unsignned)
        {
            menuPanel.SetActive(false);
            gamesPanel.SetActive(false);
            loginPanel.SetActive(true);
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
        profilePanel.SetActive(true);
    }

    public void GotoGameDetail(int game)
    {
        gameDetailPanel.SetActive(true);
        gameDetailPanel.GetComponent<GameDetailPanel>().SelectGame((Wagr.GameName)game);
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
