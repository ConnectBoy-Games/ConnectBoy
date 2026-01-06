using TMPro;
using UnityEngine;

public class StatPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text gamesPlayedText;
    [SerializeField] private TMP_Text gamesWonText;
    [SerializeField] private TMP_Text gamesLostText;
    [SerializeField] private TMP_Text winRateText;
    [SerializeField] private TMP_Text winStreakText;

    public void OnEnable()
    {
        PlayerStats stats = GameManager.instance.accountManager.playerProfile.playerStats;
        gamesPlayedText.text = stats.gamesPlayed.ToString();
        gamesWonText.text = stats.gamesWon.ToString();
        gamesLostText.text = stats.gamesLost.ToString();
        winRateText.text = stats.winRate.ToString();
        winStreakText.text = stats.winStreak.ToString();
    }
}
