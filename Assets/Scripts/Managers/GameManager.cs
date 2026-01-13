using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioManager))]
public class GameManager : MonoBehaviour
{
    /// <summary> The singleton instance of the game manager  </summary>
    public static GameManager instance;
    public static Wagr.Session gameSession;

    public AudioManager audioManager;
    public AccountManager accountManager;

    void Awake() //Singleton Manager
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        accountManager = new();
        accountManager.Setup();
        audioManager = GetComponent<AudioManager>();
    }

    public void GoToSelectedGame()
    {
        string gameName = "";
        switch(GameManager.gameSession.gameName)
        {
            case Wagr.GameName.xando:
                gameName = "XAndO";
                break;
            case Wagr.GameName.archery:
                gameName = "Archery";
                break;
            case Wagr.GameName.dotsandboxes:
                gameName = "DotsAndBoxes";
                break;
            case Wagr.GameName.fourinarow:
                gameName = "FourInARow";
                break;
            case Wagr.GameName.minigolf:
                gameName = "MiniGolf";
                break;
            case Wagr.GameName.minisoccer:
                gameName = "MiniSoccer";
                break;
        }
        SceneManager.LoadSceneAsync(gameName, LoadSceneMode.Single);
    }
}
