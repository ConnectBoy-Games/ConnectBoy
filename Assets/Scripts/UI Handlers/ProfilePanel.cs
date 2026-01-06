using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ProfilePanel : MonoBehaviour
{
    [SerializeField] TMP_Text displayName;
    [SerializeField] TMP_InputField editUsernameField;
    [SerializeField] GameObject historyPanel;
    [SerializeField] GameObject statPanel;

    [Header("Buttons")]
    [SerializeField] Button historyButton;
    [SerializeField] Button statButton;

    [Header("Colors")]
    [SerializeField] Color selectedColor;
    [SerializeField] Color deselectedColor;

    public UnityAction backAction;

    private void OnEnable()
    {
        statButton.GetComponent<Image>().color = deselectedColor;
        historyButton.GetComponent<Image>().color = deselectedColor;
        statPanel.SetActive(false);
        historyPanel.SetActive(false);
        displayName.gameObject.SetActive(true);
        editUsernameField.gameObject.SetActive(false);

        if(GameManager.instance.accountManager.loginState == LoginState.loggedIn)
        {
            displayName.text = GameManager.instance.accountManager.playerProfile.displayName;
            statButton.interactable = true;
            historyButton.interactable = true;
        }
        else
        {
            displayName.text = "Guest";
            statButton.interactable = false;
            historyButton.interactable = false;
        }
    }

    public void SetUsername()
    {
        var username = editUsernameField.text;

        if(username.Length < 3 || username.Length > 15)
        {
            NotificationDisplay.instance.DisplayMessage("Username must be between 3 and 15 characters!", NotificationType.error);
            return;
        }
        else if(CloudSaveSystem.IsNameTaken(username).Result == true)
        {
            NotificationDisplay.instance.DisplayMessage("Username is already taken!", NotificationType.error);
            return;
        }
        
        if(CloudSaveSystem.SetUsername(username).Result == true)
        {
            editUsernameField.gameObject.SetActive(false);
            displayName.gameObject.SetActive(true);
            displayName.text = username;
            NotificationDisplay.instance.DisplayMessage("Username successfully changed!", NotificationType.info);
        }
        else
        {
            NotificationDisplay.instance.DisplayMessage("Failed to set username. Please try again.", NotificationType.error);
            return;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            backAction.Invoke();
        }
    }

    public void OpenHistoryPanel()
    {
        historyPanel.SetActive(true);
        statPanel.SetActive(false);
        historyButton.GetComponent<Image>().color = selectedColor;
        statButton.GetComponent<Image>().color = deselectedColor;
    }

    public void OpenStatPanel()
    {
        statPanel.SetActive(true);
        historyPanel.SetActive(false);
        statButton.GetComponent<Image>().color = selectedColor;
        historyButton.GetComponent<Image>().color = deselectedColor;
    }

    public void GoBack()
    {
        backAction.Invoke();
    }
}
