using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSelectionPanel : MonoBehaviour
{
    [SerializeField] TMP_Text displayName;
    [SerializeField] GameObject notificationDot;

    void Start()
    {
        UpdateUsernameDisplay();
    }

    public void UpdateUsernameDisplay()
    {
        var uName = AuthenticationService.Instance.PlayerName;
        
        if(uName != null || uName != "")
        {
            displayName.text = uName;
        }
    }

    public void LoadGame(string name)
    {
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
    }
}
