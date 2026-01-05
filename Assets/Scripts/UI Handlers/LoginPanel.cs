using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;

public class LoginPanel : MonoBehaviour
{
    public UnityAction backAction;

    [SerializeField] GameObject loginButton;
    [SerializeField] GameObject usernamePanel;
    [SerializeField] TMP_InputField usernameInput;

    public void Login()
    {
        if (GameManager.instance.accountManager.Login().Result == false) //First time user
        {
            loginButton.SetActive(false);
            usernamePanel.SetActive(true);
        }
    }

    public void LoginAsGuest()
    {
        GameManager.instance.accountManager.LoginInGuestMode();
        backAction.Invoke();
    }

    public async void CreateAccount()
    {
        var username = usernameInput.text;

        var task = await GameManager.instance.accountManager.CreateAccount(AuthenticationService.Instance.PlayerId, username);

        //Account was created succesfully
        if(task == true)
        {
            GameManager.instance.accountManager.SetUserName(username, backAction);
        }
    }
    
    public void GoToTermsOfService()
    {
        Application.OpenURL("www.google.com");
    }
}
