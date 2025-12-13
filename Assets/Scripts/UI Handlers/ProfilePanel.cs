using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using UnityEngine;
using UnityEngine.Events;

public class ProfilePanel : MonoBehaviour
{
    [SerializeField] TMP_InputField displayName;
    public UnityAction backAction;

    private void OnEnable()
    {
        UpdateUsernameDisplay();
    }

    async void UpdateUsernameDisplay()
    {
        var uName = await AuthenticationService.Instance.GetPlayerNameAsync();
        displayName.text = uName;
    }

    public async void SetUsername()
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(displayName.name);
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
    }

    public void GoBack()
    {
        backAction.Invoke();
    }
}
