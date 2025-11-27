using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    private AudioManager audioManager;
    public UnityAction backAction;

    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle sfxToggle;
    [SerializeField] Toggle vibrateToggle;
    [SerializeField] Toggle themeToggle;

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
        themeToggle.interactable = false;
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

    public void ToggleTheme()
    {
        print("Not Yet Implemented: " + themeToggle.isOn);
        audioManager.PlayClickSound();
#if UNITY_EDITOR
        throw new NotImplementedException();
#endif
    }

    public void OpenTerms()
    {
        Application.OpenURL("www.google.com");
    }
}
