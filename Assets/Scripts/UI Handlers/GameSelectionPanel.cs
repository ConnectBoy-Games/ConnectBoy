using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSelectionPanel : MonoBehaviour
{
    public void LoadGame(string name)
    {
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
    }
}
