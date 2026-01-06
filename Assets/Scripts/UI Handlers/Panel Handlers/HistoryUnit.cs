using UnityEngine;
using TMPro;

public class HistoryUnit : MonoBehaviour
{
    [SerializeField] private TMP_Text gameName;
    [SerializeField] private TMP_Text wagerValue;

    [Header("Colors")]
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    public void SetDetails(string gameNameStr, int wager, bool won)
    {
        gameName.text = gameNameStr;
        wagerValue.text = wager.ToString();

        if(won)
        {
            wagerValue.color = winColor;
        }
        else
        {
            wagerValue.color = loseColor;
        }
    }
}
