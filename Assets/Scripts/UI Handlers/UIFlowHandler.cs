using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class UIFlowHandler : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject profilePanel;
    [SerializeField] private GameObject walletPanel;
    [SerializeField] private GameObject gamesPanel;
    [SerializeField] private GameObject loginPanel;

    async void Awake()
    {
        await UnityServices.InitializeAsync();

        if(GameManager.instance.accountManager.loginState == Wagr.LoginState.unsignned)
        {
            menuPanel.SetActive(false);
            gamesPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
    }

    void Start()
    {
        loginPanel.GetComponent<LoginPanel>().backAction = GotoMainMenu;
        settingsPanel.GetComponent<SettingsPanel>().backAction = GotoMainMenu;
        profilePanel.GetComponent<ProfilePanel>().backAction = GotoMainMenu;
        //walletPanel.GetComponent<WalletPanel>().backAction = GotoMainMenu;
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

    public void GotoWallet()
    {
        walletPanel.SetActive(true);
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
        walletPanel.SetActive(false);
        loginPanel.SetActive(false);
        notificationPanel.SetActive(false);
    }
}
