using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AISelectPanel : MonoBehaviour
{
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private Image sliderHandle;
    [SerializeField] private Image sliderBackground;
    [SerializeField] private Image playButton;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private Color[] difficultyColor;

    [Header("Difficulty Heads")]
    [SerializeField] private Image easyHead;
    [SerializeField] private Image mediumHead;
    [SerializeField] private Image hardHead;
    
    void OnEnable()
    {
        easyHead.canvasRenderer.SetAlpha(0);
        mediumHead.canvasRenderer.SetAlpha(0);
        hardHead.canvasRenderer.SetAlpha(0);
        SetDificulty();
    }

    public void SetDificulty()
    {
        int value = (int)difficultySlider.value;

        switch (difficultySlider.value)
        {
            case 0: //Easy mode
                difficultyText.text = "Easy";
                easyHead.CrossFadeAlpha(1, 1, true);
                mediumHead.CrossFadeAlpha(0, 1, true);
                hardHead.CrossFadeAlpha(0, 1, true);
                GameManager.gameSession.botDifficulty = BotDifficulty.low;
                break;
            case 1: //Medium mode
                difficultyText.text = "Medium";
                mediumHead.CrossFadeAlpha(1, 1, true);
                easyHead.CrossFadeAlpha(0, 1, true);
                hardHead.CrossFadeAlpha(0, 1, true);
                GameManager.gameSession.botDifficulty = BotDifficulty.medium;
                break;
            case 2: //Hard mode
                difficultyText.text = "Hard";
                hardHead.CrossFadeAlpha(1, 1, true);
                easyHead.CrossFadeAlpha(0, 1, true);
                mediumHead.CrossFadeAlpha(0, 1, true);
                GameManager.gameSession.botDifficulty = BotDifficulty.high;
                break;
        }

        difficultyText.CrossFadeColor(difficultyColor[value], 1f, true, true);
        playButton.CrossFadeColor(difficultyColor[value], 1f, true, true);
        sliderHandle.CrossFadeColor(difficultyColor[value], 1f, true, true);
        sliderBackground.CrossFadeColor(difficultyColor[value], 1f, true, true);
    }

    public void PlayGame()
    {
        //Load the actual game level and set the scene accordingly
        GameManager.instance.GoToSelectedGame();
    }
}
