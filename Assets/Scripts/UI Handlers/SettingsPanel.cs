using System;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    private AudioManager audioManager;
    public UnityAction backAction;

    [Header("Audio Settings")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle sfxToggle;
    [SerializeField] Toggle vibrateToggle;

    [Header("Account Buttons")]
    [SerializeField] GameObject logoutButton;
    [SerializeField] GameObject deleteButton;

    [Header("Prompt Menu")]
    [SerializeField] GameObject promptMenu;
    [SerializeField] Button promptButton;

    void Start()
    {
        audioManager = GameManager.instance.audioManager;
        Invoke(nameof(SetDefaultUI), 0.1f); //Delay setting the default UI
    }

    void OnEnable()
    {
        if(GameManager.instance.accountManager.loginState != LoginState.loggedIn)
        {
            logoutButton.SetActive(false);
            deleteButton.SetActive(false);
        }
        else
        {
            logoutButton.SetActive(true);
            deleteButton.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            backAction.Invoke();
        }
    }

    public void GoBack()
    {
        if(promptMenu.activeInHierarchy)
        {
            promptMenu.SetActive(false); 
        }
        else
        {
            backAction.Invoke();
        }
    }

    public void SetDefaultUI()
    {
        volumeSlider.value = audioManager.volume;
        sfxToggle.isOn = audioManager.sfx;
        vibrateToggle.isOn = audioManager.vibrate;
    }

    public void SetVolume()
    {
        audioManager.SetVolume(volumeSlider.value);
    }

    public void ToggleSfx()
    {
        audioManager.SetSfx(sfxToggle.isOn);
        audioManager.PlayClickSound();
    }

    public void ToggleVibrate()
    {
        audioManager.SetVibrate(vibrateToggle.isOn);
        audioManager.PlayClickSound();
    }

    public void LogOutButtonClicked()
    {
        promptMenu.SetActive(true);
        promptButton.onClick.RemoveAllListeners();
        promptButton.onClick.AddListener(SignOut);
    }

    public void DeleteButtonClicked()
    {
        promptMenu.SetActive(true);
        promptButton.onClick.RemoveAllListeners();
        promptButton.onClick.AddListener(DeleteAccount);
    }

    public void SignOut()
    {
        bool clearSessionToken = true;

        // Sign out of Unity Authentication, with the option to clear the session token
        AuthenticationService.Instance.SignOut(clearSessionToken);

        // Sign out of Unity Player Accounts
        PlayerAccountService.Instance.SignOut();

        GameManager.instance.accountManager.SignOut();

        //Reload the Main Menu
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
    }

    public void DeleteAccount()
    {
        Application.OpenURL(PlayerAccountService.Instance.AccountPortalUrl);
    }

    public void OpenTerms()
    {
        Application.OpenURL("https://connectboy-games.web.app/terms.html");
    }
}
