using TMPro;
using UnityEngine;

public class ScorePanel : MonoBehaviour
{
    /// <summary>A single instance of the Score Panel</summary>
    public static ScorePanel instance;

    [SerializeField] TMP_Text wagerText;

    [Header("Client Details")]
    [SerializeField] TMP_Text yourUsername;
    [SerializeField] TMP_Text yourScore;

    [Header("Other Details")]
    [SerializeField] TMP_Text otherUsername;
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

    public void SetWagerText(int wager) => wagerText.text = wager.ToString();

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
