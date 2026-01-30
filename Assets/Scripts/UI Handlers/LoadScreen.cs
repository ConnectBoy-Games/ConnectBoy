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
        GameManager.instance.GetComponent<AudioManager>().PlayWobbleSound();
    }

    public void HideScreen()
    {
        gameObject.SetActive(false);
        GameManager.instance.GetComponent<AudioManager>().PlayClickSound();
    }
}
