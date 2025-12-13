using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    private bool newUser = false; //Is the user a first timer?
    public UnityAction backAction;

    [SerializeField] GameObject loginButton;
    [SerializeField] GameObject usernamePanel;
    [SerializeField] Button submitUsername;
    [SerializeField] TMP_InputField usernameInput;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        SetupEvents();

        // Check if a cached player already exists by checking if the session token exists
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            newUser = true;
        }
        else
        {
            newUser = false;
        }


        // Register the Unity Player Accounts sign-in event handler after services initialization.
        //PlayerAccountService.Instance.SignedIn += SignInWithUnityAuth;
    }

    // Call this from a button or other application-specific trigger to begin the sign-in flow.
    public async void StartPlayerAccountsSignInAsync()
    {
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            // If the player is already signed into Unity Player Accounts, proceed directly to the Unity Authentication sign-in.
            await SignInWithUnityAuth();
        }
        else
        {
            try
            {
                // This will open the system browser and prompt the user to sign in to Unity Player Accounts
                await PlayerAccountService.Instance.StartSignInAsync();
            }
            catch (PlayerAccountsException ex)
            {
                // Compare error code to PlayerAccountsErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }
    }

    async Task SignInWithUnityAuth()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public void LoginInGuestMode()
    {
        GameManager.instance.accountManager.loginState = Wagr.LoginState.guestMode;
        backAction.Invoke();
    }

    public void SignOut(bool clearSessionToken = false)
    {
        // Sign out of Unity Authentication, with the option to clear the session token
        AuthenticationService.Instance.SignOut(clearSessionToken);

        // Sign out of Unity Player Accounts
        PlayerAccountService.Instance.SignOut();
    }

    public void GoToTermsOfService()
    {
        Application.OpenURL("www.google.com");
    }

    public string GetPlayerName()
    {
        var uname = AuthenticationService.Instance.GetPlayerNameAsync();
        return uname.Result;
    }

    public async void SetUserName(string name)
    {
        try
        {
            //var uName = AuthenticationService.Instance.GetPlayerNameAsync();
            //print(uName);

            var task = await AuthenticationService.Instance.UpdatePlayerNameAsync(usernameInput.text);
            print("Username:" + task);
            backAction.Invoke();
        }
        catch (RequestFailedException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    // Setup authentication event handlers if desired
    private void SetupEvents()
    {
        PlayerAccountService.Instance.SignedIn += () =>
        {
            Debug.Log("Player signed in successfully");
            GameManager.instance.accountManager.loginState = Wagr.LoginState.loggedIn;
            
            if(newUser)
            {
                loginButton.SetActive(false);
                usernamePanel.SetActive(true);
            }
            else
            {
                backAction.Invoke();
            }
        };

        AuthenticationService.Instance.SignedIn += () =>
        {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };

        print("Setup Events");
    }

    private void CheckStates()
    {
        // this is true if the access token exists, but it can be expired or refreshing
        Debug.Log($"Is SignedIn: {AuthenticationService.Instance.IsSignedIn}");

        // this is true if the access token exists and is valid/has not expired
        Debug.Log($"Is Authorized: {AuthenticationService.Instance.IsAuthorized}");

        // this is true if the access token exists but has expired
        Debug.Log($"Is Expired: {AuthenticationService.Instance.IsExpired}");
    }
}
