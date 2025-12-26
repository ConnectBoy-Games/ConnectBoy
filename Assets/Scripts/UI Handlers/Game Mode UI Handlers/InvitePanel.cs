using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvitePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField wagerField;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private Button playButton;

    private void OnEnable()
    {
        wagerField.text = "";
        usernameField.text = "";

        /*
        if(GameManager.instance.accountManager.loginState != LoginState.loggedIn)
        {
            wagerField.interactable = false;
            usernameField.interactable = false;
        }
        */
    }

    public void SubmitUsername()
    {
        //TODO: Confirm if username is valid and then allow the player in
        //Else, diplay a warning message that no such user exists
    }
}
