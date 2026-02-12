using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Wagr;

public class ProfilePanel : MonoBehaviour
{
    private Player localProfile;
    public UnityAction backAction;

    [Header("Profile")]
    [SerializeField] Image dpImage;
    [SerializeField] TMP_Text displayName;
    [SerializeField] TMP_InputField editUsernameField;

    [Header("Face Panel")]
    [SerializeField] GameObject facePanel;

    [Header("Stats")]
    [SerializeField] Button historyButton;
    [SerializeField] Button statButton;
    [SerializeField] GameObject historyPanel;
    [SerializeField] GameObject statPanel;

    [Header("Colors")]
    [SerializeField] Color selectedColor;
    [SerializeField] Color deselectedColor;

    private void OnEnable()
    {
        localProfile = GameManager.instance.accountManager.playerProfile;

        statPanel.SetActive(false);
        historyPanel.SetActive(false);

        statButton.GetComponent<Image>().color = deselectedColor;
        historyButton.GetComponent<Image>().color = deselectedColor;

        displayName.gameObject.SetActive(true);
        editUsernameField.gameObject.SetActive(false);

        if (GameManager.instance.accountManager.loginState == LoginState.loggedIn)
        {
            displayName.text = GameManager.instance.accountManager.playerProfile.Name;
            dpImage.sprite = GameManager.instance.faceManager.GetFace(GameManager.instance.accountManager.playerProfile.DpIndex);
            statButton.interactable = true;
            historyButton.interactable = true;
        }
        else
        {
            displayName.text = "Guest";
            dpImage.sprite = GameManager.instance.faceManager.GetFace(-1);
            statButton.interactable = false;
            historyButton.interactable = false;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) GoBack();
    }

    public async void SetUsername()
    {
        var username = editUsernameField.text;
        if (username.Length < 3 || username.Length > 15)
        {
            NotificationDisplay.instance.DisplayMessage("Username must be between 3 and 15 characters!", NotificationType.error);
            return;
        }

        var isNameTaken = await CloudSaveSystem.IsNameTaken(username);
        if (isNameTaken)
        {
            NotificationDisplay.instance.DisplayMessage("Username is already taken!", NotificationType.error);
            return;
        }

        var result = await CloudSaveSystem.SetUsername(username);
        if (result) //Succesfully set username!
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

    public void SetFace()
    {
        //TODO: Set the face and save it to the player's cloud profile

        facePanel.SetActive(false);
    }

    public void OpenFacePanel()
    {
        facePanel.SetActive(true);
        FaceSelect.dpIndex = GameManager.instance.accountManager.playerProfile.DpIndex;
        facePanel.GetComponent<FaceSelect>().UpdateUI();
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
        //Check if we can update the profile details

        if (facePanel.activeSelf)
        {
            facePanel.SetActive(false);
        }
        else if(editUsernameField.gameObject.activeSelf)
        {
            editUsernameField.gameObject.SetActive(false);
        }
        else
        {
            backAction.Invoke();
        }
    }
}
