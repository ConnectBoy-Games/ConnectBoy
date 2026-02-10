using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoginPanel : MonoBehaviour
{
    public static int dpIndex = 0;
    
    public UnityAction backAction;

    [SerializeField] GameObject loginButton;
    [SerializeField] GameObject guestButton;
    [SerializeField] GameObject usernamePanel;
    [SerializeField] TMP_InputField usernameInput;

    public async void OnEnable()
    {
        await UnityServices.InitializeAsync(); //Initialize Unity Services

        //Try logging in the moment the login page is shown
        // Check if a cached player already exists by checking if the session token exists
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            Invoke(nameof(Login), 1f);
        }
    }

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }

    void GoBack()
    {
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
    }

    public async void Login()
    {
        LoadScreen.instance.ShowScreen("Logging In!"); //Show the load screen
        await GameManager.instance.accountManager.Login();
        GameManager.instance.accountManager.onProfileLoaded += OnProfileLoaded;
    }

    public void OnProfileLoaded(bool profileExists)
    {
        LoadScreen.instance.HideScreen();
        if (profileExists == false)
        {
            loginButton.SetActive(false);
            guestButton.SetActive(false);
            usernamePanel.SetActive(true);
        }
        else
        {
            GameManager.instance.GetComponent<AudioManager>().PlayAcceptSound();
            backAction?.Invoke();
        }
    }

    public void LoginAsGuest()
    {
        GameManager.instance.accountManager.LoginInGuestMode();
        backAction.Invoke();
    }

    public async void CreateAccount()
    {
        var username = usernameInput.text;

        if (username.Length < 3 || username.Length > 10)
        {
            NotificationDisplay.instance.DisplayMessage("Username must be between 3 and 15 characters!", NotificationType.error);
            return;
        }

        LoadScreen.instance.ShowScreen("Creating Acconut!"); //Show the load screen
        await GameManager.instance.accountManager.CreateAccount(username, dpIndex);
        GameManager.instance.accountManager.onAccountCreated += OnCreatedAccount;
    }

    public void OnCreatedAccount(bool result)
    {
        LoadScreen.instance.HideScreen(); //Hide the load screen
        if (result == true) //Account was created succesfully
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
        Application.OpenURL("https://connectboy-games.web.app/terms.html");
    }
}
