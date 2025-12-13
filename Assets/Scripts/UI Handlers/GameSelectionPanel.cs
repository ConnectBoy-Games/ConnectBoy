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

    }

    private void OnEnable()
    {
        UpdateUsernameDisplay();
    }

    async void UpdateUsernameDisplay()
    {
        var uName = await AuthenticationService.Instance.GetPlayerNameAsync();
        displayName.text = uName;
    }

    public void LoadGame(string name)
    {
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
    }
}
