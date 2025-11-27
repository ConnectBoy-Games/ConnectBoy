using UnityEngine;

public class UIFlowHandler : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject profilePanel;
    [SerializeField] private GameObject walletPanel;
    [SerializeField] private GameObject gamesPanel;
    [SerializeField] private GameObject loginPanel;

    [SerializeField] private Transition splitTransition;

    void Start()
    {
        loginPanel.GetComponent<LoginPanel>().backAction = GotoMainMenu;
        settingsPanel.GetComponent<SettingsPanel>().backAction = GotoMainMenu;
        profilePanel.GetComponent<ProfilePanel>().backAction = GotoMainMenu;
        walletPanel.GetComponent<WalletPanel>().backAction = GotoMainMenu;
    }

    public void PlayTransition()
    {
        splitTransition.gameObject.SetActive(true);
        splitTransition.ClearCallbacks();
        splitTransition.playbackEnded.AddListener(() => { splitTransition.gameObject.SetActive(false); });
        splitTransition.Play();
    }

    public void GotoSettings()
    {
        PlayTransition();
        settingsPanel.SetActive(true);
    }

    public void GotoProfile()
    {
        PlayTransition();
        profilePanel.SetActive(true);
    }

    public void GotoWallet()
    {
        PlayTransition();
        walletPanel.SetActive(true);
    }

    public void GotoMainMenu()
    {
        PlayTransition();
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
    }
}
