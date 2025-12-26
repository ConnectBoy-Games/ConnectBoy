using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public UnityAction backAction;

    [SerializeField] GameObject loginButton;
    [SerializeField] GameObject usernamePanel;
    [SerializeField] Button submitUsername;
    [SerializeField] TMP_InputField usernameInput;

    public void GoToTermsOfService()
    {
        Application.OpenURL("www.google.com");
    }
}
