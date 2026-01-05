using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;

using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AccountManager
{
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
        PlayerAccountService.Instance.SignedIn += () =>
        {
            Debug.Log("Player signed in successfully");
            GameManager.instance.accountManager.loginState = LoginState.loggedIn;
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
    public async Task<bool> Login()
    {
        // Check if a cached player already exists by checking if the session token exists
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        }
        else //Sign in the player from the browser
        {
            try
            {
                await PlayerAccountService.Instance.StartSignInAsync();
            }
            catch (Exception ex)
            {
                NotificationDisplay.instance.DisplayMessage(ex.Message, NotificationType.error);
            }
        }

        //TODO: Check if the account already exists
        try
        {
            var id = AuthenticationService.Instance.PlayerId;

            //CloudSaveService.Instance.Data.Player.LoadAsync(id);
        }
        catch (Exception ex)
        {
            NotificationDisplay.instance.DisplayMessage(ex.Message, NotificationType.error);
        }

        return false;
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
        //Check if the username is in use
        if (CheckUsername(username))
        {
            NotificationDisplay.instance.DisplayMessage("Username is already in use", time: 3);
            return false;
        }
        else
        {
            try
            {
                Profile playerProfile = new Profile(AuthenticationService.Instance.PlayerId, username);
                var data = new Dictionary<string, object> { { AuthenticationService.Instance.PlayerId, playerProfile } };

                //await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            }
            catch (System.Exception ex)
            {
                NotificationDisplay.instance.DisplayMessage(ex.Message, NotificationType.error);
            }
        }
        return true;
    }
    
    /// <summary>Returns true if the username is in use</summary>
    public bool CheckUsername(string name)
    {
        return false;
    }

    public string GetPlayerName()
    {
        var uname = AuthenticationService.Instance.GetPlayerNameAsync();
        return uname.Result;
    }

    public async void SetUserName(string name, UnityAction callback)
    {
        var temp = await CloudSaveSystem.SetUsername(name);
        if(temp == true)
        {
            NotificationDisplay.instance.DisplayMessage("Username set successfully!", time: 2);
            callback?.Invoke();
        }
        else
        {
            NotificationDisplay.instance.DisplayMessage("Failed to set username.", NotificationType.error, 3);
        }
    }

    public bool CheckUsername()
    {
        return false;
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
