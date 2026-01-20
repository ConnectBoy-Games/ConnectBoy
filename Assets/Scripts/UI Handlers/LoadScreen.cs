using UnityEngine;

public class LoadScreen : MonoBehaviour
{
    /// <summary> A single instance of the Load Screen </summary>
    public static LoadScreen instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowScreen()
    {
        gameObject.SetActive(true);
    }

    public void HideScreen()
    {
        gameObject.SetActive(false);
    }
}
