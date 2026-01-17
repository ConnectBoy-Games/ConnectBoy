using TMPro;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;

public class LoginPanel : MonoBehaviour
{
    public UnityAction backAction;

    [SerializeField] GameObject loginButton;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject usernamePanel;
    [SerializeField] TMP_InputField usernameInput;

    public async void Login()
    {
        await GameManager.instance.accountManager.Login();

        GameManager.instance.accountManager.onProfileLoaded += OnProfileLoaded;
    }

    public void OnProfileLoaded(bool profileExists)
    {
        Debug.Log("Profile exists: " + profileExists);
        if (profileExists == false)
        {
            loginButton.SetActive(false);
            usernamePanel.SetActive(true);
        }
        else
        {
            loginButton.SetActive(false);
            continueButton.SetActive(true);
        }
    }

    public void LoginAsGuest()
    {
        GameManager.instance.accountManager.LoginInGuestMode();
        backAction.Invoke();
    }


    public void ContinueButton()
    {
        backAction?.Invoke();
    }

    public async void CreateAccount()
    {
        var username = usernameInput.text;

        if(username.Length < 3 || username.Length > 15)
        {
            NotificationDisplay.instance.DisplayMessage("Username must be between 3 and 15 characters!", NotificationType.error);
            return;
        }
        
        var task = await GameManager.instance.accountManager.CreateAccount(AuthenticationService.Instance.PlayerId, username);

        //Account was created succesfully
        if(task == true)
        {
            backAction?.Invoke();
        }
        else
        {
            NotificationDisplay.instance.DisplayMessage("Account could not be created!", NotificationType.error);
        }
    }
    
    public void GoToTermsOfService()
    {
        Application.OpenURL("https://www.google.com");
    }
}
