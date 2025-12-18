using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ProfilePanel : MonoBehaviour
{
    [SerializeField] TMP_InputField displayName;
    public UnityAction backAction;

    private void OnEnable()
    {
        UpdateUsernameDisplay();
    }

    public void UpdateUsernameDisplay()
    {
        var uName = AuthenticationService.Instance.PlayerName;
        displayName.text = uName;
    }

    public async void SetUsername()
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(displayName.name);
        await AuthenticationService.Instance.GetPlayerNameAsync();
    }

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            backAction.Invoke();
        }
    }

    public void SignOut(bool clearSessionToken = false)
    {
        // Sign out of Unity Authentication, with the option to clear the session token
        AuthenticationService.Instance.SignOut(clearSessionToken);

        // Sign out of Unity Player Accounts
        PlayerAccountService.Instance.SignOut();
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
        GameManager.instance.accountManager.loginState = LoginState.unsignned;
    }

    public void GoBack()
    {
        backAction.Invoke();
    }
}
