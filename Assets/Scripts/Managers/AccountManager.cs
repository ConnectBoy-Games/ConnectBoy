using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

[Serializable]
public class AccountManager
{
    public LoginState loginState { private set; get; }
    public PlayerStats playerStats { private set; get; }
    public Profile playerProfile { private set; get; }

    public async void Setup()
    {
        await UnityServices.InitializeAsync(); //Initialize Unity Services
        loginState = LoginState.unsignned;
    }

    public void Login()
    {
        // Check if a cached player already exists by checking if the session token exists
        // AuthenticationService.Instance.SessionTokenExists
    }

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
        GameManager.instance.accountManager.loginState = LoginState.guestMode;
    }

    public void SignOut(bool clearSessionToken = false)
    {
        // Sign out of Unity Authentication, with the option to clear the session token
        AuthenticationService.Instance.SignOut(clearSessionToken);

        // Sign out of Unity Player Accounts
        PlayerAccountService.Instance.SignOut();
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

            var task = await AuthenticationService.Instance.UpdatePlayerNameAsync("usernameInput.text");
            //print("Username:" + task);
        }
        catch (RequestFailedException ex)
        {
            Debug.Log(ex.Message);
        }
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

    public bool CheckUsername()
    {
        return false;
    }

    public void EditUsername()
    {

    }

    public void DeleteAccount()
    {

    }

    //Template for setting up authentication event handlers if desired
    private void SetupEvents()
    {
        PlayerAccountService.Instance.SignedIn += () =>
        {
            Debug.Log("Player signed in successfully");
            GameManager.instance.accountManager.loginState = LoginState.loggedIn;

            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        };

        AuthenticationService.Instance.SignedIn += () =>
        {

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
    }

    internal void LogOut()
    {
        throw new NotImplementedException();
    }
}
