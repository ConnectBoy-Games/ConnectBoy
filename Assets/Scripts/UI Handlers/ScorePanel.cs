using TMPro;
using UnityEngine;

public class ScorePanel : MonoBehaviour
{
    /// <summary>A single instance of the Score Panel</summary>
    public static ScorePanel instance;

    [Header("Usernames")]
    [SerializeField] TMP_Text yourUsername;
    [SerializeField] TMP_Text otherUsername;

    [Header("Scores")]
    [SerializeField] TMP_Text yourScore;
    [SerializeField] TMP_Text otherScore;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnEnable()
    {
        if(GameManager.gameSession.gameMode == GameMode.vsBot && GameManager.gameSession.gameName == Wagr.GameName.xando)
        {
            yourScore.gameObject.SetActive(false);
            otherScore.gameObject.SetActive(false);
        }
    }

    public void SetUsernames(string yours, string others)
    {
        yourUsername.text = yours;
        otherUsername.text = others;
    }

    public void UpdateScore(string yours, string others)
    {
        yourScore.text = yours;
        otherScore.text = others;
    }
}
