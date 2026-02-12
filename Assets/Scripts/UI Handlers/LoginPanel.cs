using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public UnityAction backAction;

    [Header("Login Flow Panels")]
    [SerializeField] GameObject loginButtonPanel;
    [SerializeField] GameObject createAccountPanel;
    [SerializeField] GameObject continuePanel;

    [Header("Login Buttons")]
    [SerializeField] Button loginButton;
    [SerializeField] Button signUpButton;
    [SerializeField] Button guestButton;

    [Header("Create Account Fields")]
    [SerializeField] TMP_InputField usernameInput;

    [Header("Profile Fields")]
    [SerializeField] Image dpImage;
    [SerializeField] TMP_Text username;

    public async void Start()
    {
        await UnityServices.InitializeAsync(); //Initialize Unity Services
        GameManager.instance.accountManager.onProfileLoaded += OnProfileLoaded;
        GameManager.instance.accountManager.onAccountCreated += OnCreatedAccount;
    }

    public async void OnEnable()
    {
        //Try logging in the moment the login page is shown
        if (AuthenticationService.Instance.SessionTokenExists) //Check if a cached player already exists by checking if the session token exists
        {
            loginButton.interactable = false;
            signUpButton.interactable = false;
            guestButton.interactable = false;
            Invoke(nameof(Login), 0.5f);
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }

    void GoBack()
    {
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
    }

    /// <summary>Used to try logging in existing players!</summary>
    public async void Login()
    {
        LoadScreen.instance.ShowScreen("Logging In!"); //Show the load screen
        await GameManager.instance.accountManager.Login(); //After login, the OnProfileLoaded function will be called
    }

    /// <summary>Used to register new players</summary>
    public void SignUp()
    {
        loginButtonPanel.SetActive(false);
        createAccountPanel.SetActive(true);
    }

    /// <summary>Login as a guest user</summary>
    public void LoginAsGuest()
    {
        GameManager.instance.accountManager.LoginInGuestMode();
        backAction.Invoke();
    }

    /// <summary>This is called when the continue panel is clicked</summary>
    public void GoToGames()
    {
        backAction.Invoke();
    }

    /// <summary>Function that handles account creation</summary>
    public async void CreateAccount()
    {
        var username = usernameInput.text;

        if (username.Length < 3 || username.Length > 10) //Make sure the username is the right number of characters
        {
            NotificationDisplay.instance.DisplayMessage("Username must be between 3 and 15 characters!", NotificationType.error);
            return;
        }

        var isNameUsed = await CloudSaveSystem.IsNameTaken(username); //Check if the username is in use

        if (isNameUsed)
        {
            NotificationDisplay.instance.DisplayMessage("Username is already in use", NotificationType.error, 10f);
            return;
        }

        LoadScreen.instance.ShowScreen("Creating Account!"); //Show the load screen
        await GameManager.instance.accountManager.CreateAccount(username, FaceSelect.dpIndex); //The OnCreatedAccount function will be called after this function finishes
    }

    /// <summary>Set the profile details for the continue panel</summary>
    private void SetProfileDetails()
    {
        var profile = GameManager.instance.accountManager.playerProfile;
        username.text = profile.Name;
        dpImage.sprite = GameManager.instance.faceManager.GetFace(profile.DpIndex);
    }

    /// <summary>Called after an attempt to load the profile</summary>
    /// <param name="profileExists">Does the profile exist or not?</param>
    public void OnProfileLoaded(bool profileExists)
    {
        LoadScreen.instance.HideScreen(); //Hide the load screen

        if (profileExists == false) //No existing profile was found, show the sign up page
        {
            SignUp();
        }
        else //The profile was loaded succesfully, show the continue page
        {
            GameManager.instance.GetComponent<AudioManager>().PlayAcceptSound();
            SetProfileDetails();
            continuePanel.SetActive(true);
        }
    }

    /// <summary>Called after a create account request is made</summary>
    /// <param name="result">Was creation successful or not?</param>
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

    /// <summary>Opens the CB terms and condition site</summary>
    public void GoToTermsOfService()
    {
        GameManager.instance.accountManager.OpenTerms();
    }
}
