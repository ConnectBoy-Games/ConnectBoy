using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] Button enableLoginPanel;
    [SerializeField] Button enableSignUpPanel;

    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject signupPanel;

    [Header("Login Fields")]
    [SerializeField] TMP_InputField loginEmailField;
    [SerializeField] TMP_InputField loginPasswordField;

    [Header("SignUp Fields")]
    [SerializeField] TMP_InputField signUpUsernameField;
    [SerializeField] TMP_InputField signUpEmailField;
    [SerializeField] TMP_InputField signUpPasswordField;

    public UnityAction backAction;

    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
            SetupAuthenticators();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            //TODO: Display errors!
        }
    }

    void Start()
    {
        //TODO: Check if we are signed in
    }

    #region UI Handling
    public void EnableLoginPanel()
    {
        signupPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    public void EnableSignUpPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
    }

    public void LoginButtonClicked()
    {
        string email = loginEmailField.text;
        string password = loginPasswordField.text;

        var task = SignInWithUsernamePasswordAsync(email, password);
        if(task.IsCompletedSuccessfully)
        {
            Debug.Log("Logged in successfully");
        }
    }

    public void CreateAccountButtonClicked()
    {
        string email = signUpEmailField.text;
        string password = signUpPasswordField.text;
        string username = signUpUsernameField.text;
        var task = SignUpWithUsernamePasswordAsync(email, password);

        if(task.IsCompletedSuccessfully)
        {
            Debug.Log("Created account succesfully");
        }
    }

    public void ClearAllFields()
    {
        loginEmailField.text = "";
        loginPasswordField.text = "";

        signUpUsernameField.text = "";
        signUpEmailField.text = "";
        signUpPasswordField.text = "";
    }

    public void LoginWithGoogle()
    {
        /*
         * if(login == successful)
         * {
         *      loginState = LoginState.loggedIn;
         * }
         */
    }

    public void LoginWithApple()
    {
        /*
         * if(login == successful)
         * {
         *      loginState = LoginState.loggedIn;
         * }
         */
    }

    public void LoginInGuestMode()
    {
        GameManager.instance.accountManager.loginState = Wagr.LoginState.guestMode;
        backAction.Invoke();
    }
    #endregion


    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");
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

    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
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

    async Task UpdatePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePasswordAsync(currentPassword, newPassword);
            Debug.Log("Password updated.");
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

    public string GetPlayerName()
    {
        var uname = AuthenticationService.Instance.GetPlayerNameAsync();
        return uname.Result;
    }

    public void SetPlayerName(string name)
    {
        AuthenticationService.Instance.UpdatePlayerNameAsync(name);
    }

    public void SetupAuthenticators()
    {
    }

    // Setup authentication event handlers if desired
    private void SetupEvents()
    {
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
    }

}
