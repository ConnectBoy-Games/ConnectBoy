using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;

using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AccountManager
{
    public UnityAction<bool> onProfileLoaded;
    public LoginState loginState { private set; get; }
    public Profile playerProfile { private set; get; }

    public async void Setup()
    {
        await UnityServices.InitializeAsync(); //Initialize Unity Services
        loginState = LoginState.unsignned;
        SetupEvents();
    }
    
    private void SetupEvents()
    {
        PlayerAccountService.Instance.SignedIn += async () =>
        {
            //Sign in to authentication service with the access token from player accounts
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);

            GameManager.instance.accountManager.loginState = LoginState.loggedIn;

            //Load the player profile
            var loaded  = await GameManager.instance.accountManager.LoadProfile();
            onProfileLoaded?.Invoke(loaded);
        };

        PlayerAccountService.Instance.SignInFailed += (RequestFailedException ex) =>
        {
            NotificationDisplay.instance.DisplayMessage(ex.Message, NotificationType.error);
            GameManager.instance.accountManager.loginState = LoginState.unsignned;
        };

        PlayerAccountService.Instance.SignedOut += () =>
        {
            GameManager.instance.accountManager.loginState = LoginState.unsignned;
        };

        /*
        // Shows how to get a playerID
        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        // Shows how to get an access token
        Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        */
    }

    #region Authentication
    /// <summary>Return true if the player has an existing account</summary>
    public async Task Login()
    {
        // Check if a cached player already exists by checking if the session token exists
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        else if (PlayerAccountService.Instance.IsSignedIn)
        {
            Debug.Log("Signing in player with cached session token");
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        }
        else //Sign in the player from the browser
        {
            try
            {
                Debug.Log("Signing in player from the browser");
                await PlayerAccountService.Instance.StartSignInAsync();
                //await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to sign in player with Unity Player Accounts: " + ex.Message);
                NotificationDisplay.instance.DisplayMessage(ex.Message, NotificationType.error);
            }
        }
    }

    public async Task<bool> LoadProfile()
    {
        var id = AuthenticationService.Instance.PlayerId;
        playerProfile = await CloudSaveSystem.RetrieveSpecificData<Profile>(id);

        if (playerProfile == default)
        {
            return false;
        }

        return true;
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
    #endregion

    #region Profile
    public async Task<bool> CreateAccount(string playerId, string username)
    {
        if(CloudSaveSystem.IsNameTaken(username).Result == true) //Check if the username is in use
        {
            NotificationDisplay.instance.DisplayMessage("Username is already in use", time: 3);
            return false;
        }
        else
        {
            try
            {
                Profile prof = new(AuthenticationService.Instance.PlayerId, username);
                CloudSaveSystem.SaveSpecificData<Profile>(AuthenticationService.Instance.PlayerId, prof).Wait();
                CloudSaveSystem.SetUsername(username).Wait();
            }
            catch (System.Exception ex)
            {
                NotificationDisplay.instance.DisplayMessage(ex.Message, NotificationType.error);
                return false;
            }
        }
        return true;
    }
    #endregion

    private void CheckStates()
    {
        // this is true if the access token exists, but it can be expired or refreshing
        Debug.Log($"Is SignedIn: {AuthenticationService.Instance.IsSignedIn}");

        // this is true if the access token exists and is valid/has not expired
        Debug.Log($"Is Authorized: {AuthenticationService.Instance.IsAuthorized}");

        // this is true if the access token exists but has expired
        Debug.Log($"Is Expired: {AuthenticationService.Instance.IsExpired}");
    }

    public void DeleteAccount()
    {

    }
}
