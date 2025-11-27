using UnityEngine;
using UnityEngine.Events;

public class LoginPanel : MonoBehaviour
{
    public UnityAction backAction;

    void Start()
    {
        //TODO: Check if we are signed in
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
}
