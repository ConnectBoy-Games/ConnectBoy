using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioManager))]
public class GameManager : MonoBehaviour
{
    /// <summary> The singleton instance of the game manager  </summary>
    public static GameManager instance;

    /// <summary>Information about the current online session</summary>
    public static Wagr.Session gameSession;

    /// <summary>The current game mode being run</summary>
    public static GameMode gameMode;

    /// <summary>The difficulty of the bot in the bot mode</summary>
    public static BotDifficulty botDifficulty = BotDifficulty.low;

    public AudioManager audioManager;
    public AccountManager accountManager;
    public FaceManager faceManager;

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
        switch(gameSession.gameName)
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
