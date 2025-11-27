using UnityEngine;

[RequireComponent(typeof(AudioManager))]
public class GameManager : MonoBehaviour
{
    /// <summary> The singleton instance of the game manager  </summary>
    public static GameManager instance;

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

        print("Sets up Game Manager!!!");
    }
}
