using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;
using Wagr;

[Serializable]
public class AccountManager
{
    public UnityAction<bool> onProfileLoaded;
    public UnityAction<bool> onAccountCreated;

    public LoginState loginState { private set; get; }
    public Player playerProfile { private set; get; }
    public PlayerStats playerStats { private set; get; }

    public async void Setup()
    {
        await UnityServices.InitializeAsync(); //Initialize Unity Services
        loginState = LoginState.unsigned;
        SetupEvents();
    }

    private void SetupEvents()
    {
        //Sign in to authentication service with the access token from player accounts
        PlayerAccountService.Instance.SignedIn += async () =>
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        };

        AuthenticationService.Instance.SignedIn += async () =>
        {
            GameManager.instance.accountManager.loginState = LoginState.loggedIn;

            var loaded = await GameManager.instance.accountManager.LoadProfile(); //Load the player profile
            onProfileLoaded?.Invoke(loaded);
        };

        PlayerAccountService.Instance.SignInFailed += (RequestFailedException ex) =>
        {
            Debug.Log("Player signin failed! " + ex.Message);
            NotificationDisplay.instance.DisplayMessage("Player Sign In failed! " + ex.Message, NotificationType.error);
            GameManager.instance.accountManager.loginState = LoginState.unsigned;
        };

        PlayerAccountService.Instance.SignedOut += () =>
        {
            Debug.Log("Player Signed Out!");
            NotificationDisplay.instance.DisplayMessage("You Have Been Signed Out!");
            GameManager.instance.accountManager.loginState = LoginState.unsigned;
        };
    }

    public async Task Login()
    {
        try
        {
            //Check if a cached player already exists and try signing in with that session token
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            //If the player is already signed in with Player Accounts, sign them in to Authentication with the access token
            else if (PlayerAccountService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            }
            else //Sign in the player from the browser
            {
                await PlayerAccountService.Instance.StartSignInAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to sign in player with Unity Player Accounts: " + ex.Message);
            NotificationDisplay.instance.DisplayMessage(ex.Message, NotificationType.error);
        }
    }

    public async Task<bool> LoadProfile()
    {
        var id = AuthenticationService.Instance.PlayerId;
        try
        {
            var args = new Dictionary<string, object>
            {
                { "targetId", id },
                { "key", id }
            };

            // Call the Cloud Code function
            var response = await CloudCodeService.Instance.CallEndpointAsync<CloudProfileGetProxy>("GetProfile", args);

            if (response.success)
            {
                playerProfile = response.data;
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get profile: {e.Message}");
            NotificationDisplay.instance.DisplayMessage($"Could not fetch public data: {e.Message}", NotificationType.error);
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

    public async Task CreateAccount(string username, int dpIndex)
    {
        try
        {
            Player player = new(AuthenticationService.Instance.PlayerId, username, dpIndex);
            PlayerStats stats = new();

            await CloudSaveSystem.SetProfile(AuthenticationService.Instance.PlayerId, player);
            await CloudSaveSystem.SaveSpecificData("stats", stats);
            await CloudSaveSystem.SetUsername(username);
            onAccountCreated?.Invoke(true);
        }
        catch (Exception ex)
        {
            onAccountCreated?.Invoke(false);
            NotificationDisplay.instance.DisplayMessage(ex.Message, NotificationType.error);
        }
    }

    private void CheckStates()
    {
        /*
        // Shows how to get a playerID
        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        // Shows how to get an access token
        Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        */

        // this is true if the access token exists, but it can be expired or refreshing
        Debug.Log($"Is SignedIn: {AuthenticationService.Instance.IsSignedIn}");

        // this is true if the access token exists and is valid/has not expired
        Debug.Log($"Is Authorized: {AuthenticationService.Instance.IsAuthorized}");

        // this is true if the access token exists but has expired
        Debug.Log($"Is Expired: {AuthenticationService.Instance.IsExpired}");
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
