using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AISelectPanel : MonoBehaviour
{
    public Wagr.GameName gameName;

    void OnEnable()
    {
        SetDificulty();
    }

    [SerializeField] private Slider difficultySlider;
    [SerializeField] private Image sliderHandle;
    [SerializeField] private Image sliderBackground;
    [SerializeField] private Image playButton;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private Color[] difficultyColor;

    public void SetDificulty()
    {
        int value = (int)difficultySlider.value;

        switch (difficultySlider.value)
        {
            case 0: //Easy mode
                difficultyText.text = "Easy";
                break;
            case 1: //Medium mode
                difficultyText.text = "Medium";
                break;
            case 2: //Hard mode
                difficultyText.text = "Hard";
                break;
        }

        difficultyText.CrossFadeColor(difficultyColor[value], 1f, true, true);
        playButton.CrossFadeColor(difficultyColor[value], 1f, true, true);
        sliderHandle.CrossFadeColor(difficultyColor[value], 1f, true, true);
        sliderBackground.CrossFadeColor(difficultyColor[value], 1f, true, true);
    }

    public void PlayGame()
    {
        //TODO: Go to actual game levl and set the scene accordingly
        SceneManager.LoadSceneAsync(gameName.ToString(), LoadSceneMode.Single);
    }
}
