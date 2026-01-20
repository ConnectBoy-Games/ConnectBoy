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

    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle sfxToggle;
    [SerializeField] Toggle vibrateToggle;

    void Start()
    {
        audioManager = GameManager.instance.audioManager;
        Invoke(nameof(SetDefaultUI), 0.2f); //Delay setting the default UI
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
        backAction.Invoke();
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

    public void SignOut(bool clearSessionToken = false)
    {
        // Sign out of Unity Authentication, with the option to clear the session token
        AuthenticationService.Instance.SignOut(clearSessionToken);

        // Sign out of Unity Player Accounts
        PlayerAccountService.Instance.SignOut();
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
        GameManager.instance.accountManager.SignOut();
    }

    public void OnDestroy()
    {
        //TODO: Actually delete the user account from backend
        SignOut(true);        
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
