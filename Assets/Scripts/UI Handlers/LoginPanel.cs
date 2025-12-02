using TMPro;
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

    void Awake()
    {
        SetupAuthenticators();
    }

    void Start()
    {
        enableSignUpPanel.GetComponent<Image>().CrossFadeAlpha(0.5f, 0.1f, true);
        //TODO: Check if we are signed in
    }

    #region UI Handling
    public void EnableLoginPanel()
    {
        signupPanel.SetActive(false);
        loginPanel.SetActive(true);

        enableLoginPanel.GetComponent<Image>().CrossFadeAlpha(1f, 0.5f, true);
        enableSignUpPanel.GetComponent<Image>().CrossFadeAlpha(0.5f, 0.5f, true);
    }

    public void EnableSignUpPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);

        enableSignUpPanel.GetComponent<Image>().CrossFadeAlpha(1f, 0.5f, true);
        enableLoginPanel.GetComponent<Image>().CrossFadeAlpha(0.5f, 0.5f, true);
    }

    public void LoginButtonClicked()
    {
        string email = loginEmailField.text;
        string password = loginPasswordField.text;

        //TODO: Login with email and password
    }

    public void CreateAccountButtonClicked()
    {
        string email = signUpEmailField.text;
        string password = signUpPasswordField.text;
        string username = signUpUsernameField.text;
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

    public void SetupAuthenticators()
    {

    }
}
