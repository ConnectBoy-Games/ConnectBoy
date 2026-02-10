using TMPro;
using UnityEngine;

public class LoadScreen : MonoBehaviour
{
    /// <summary> A single instance of the Load Screen </summary>
    public static LoadScreen instance;
    [SerializeField] TMP_Text textObject;

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

    public void ShowScreen(string text)
    {
        gameObject.SetActive(true);
        textObject.text = text;
        GameManager.instance.GetComponent<AudioManager>().PlayWobbleSound();
    }

    public void HideScreen()
    {
        gameObject.SetActive(false);
        GameManager.instance.GetComponent<AudioManager>().PlayClickSound();
    }
}
