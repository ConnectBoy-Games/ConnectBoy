using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;

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
        PlayerAccountService.Instance.SignedIn += async () =>
        {
            //Sign in to authentication service with the access token from player accounts
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
            Debug.Log("Player signin failed!");
            NotificationDisplay.instance.DisplayMessage(ex.Message, NotificationType.error);
            GameManager.instance.accountManager.loginState = LoginState.unsigned;
        };

        PlayerAccountService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out!");
            GameManager.instance.accountManager.loginState = LoginState.unsigned;
        };

        /*
        // Shows how to get a playerID
        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        // Shows how to get an access token
        Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        */
    }

    #region Authentication

    public async Task Login()
    {
        // Check if a cached player already exists by checking if the session token exists
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        else if (PlayerAccountService.Instance.IsSignedIn)
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
                Debug.LogError("Failed to sign in player with Unity Player Accounts: " + ex.Message);
                NotificationDisplay.instance.DisplayMessage(ex.Message, NotificationType.error);
            }
        }
    }

    public async Task<bool> LoadProfile()
    {
        var id = AuthenticationService.Instance.PlayerId;
        playerProfile = await CloudSaveSystem.RetrieveSpecificData<Player>(id);

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
    public async Task CreateAccount(string username)
    {
        if (await CloudSaveSystem.IsNameTaken(username)) //Check if the username is in use
        {
            NotificationDisplay.instance.DisplayMessage("Username is already in use", NotificationType.warning, 5f);
        }
        else
        {
            try
            {
                Player player = new(AuthenticationService.Instance.PlayerId, username);
                PlayerStats stats = new();

                await CloudSaveSystem.SaveSpecificData(AuthenticationService.Instance.PlayerId, player);
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
        Application.OpenURL(PlayerAccountService.Instance.AccountPortalUrl);
    }
}
