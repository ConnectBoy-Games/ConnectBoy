using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameDetailPanel : MonoBehaviour
{
    public UnityAction backAction;

    public void GoBack()
    {
        backAction();
    }

    void FixedUpdate()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            backAction();
        }
    }

    [Header("Game Detail UI")]
    [SerializeField] private Image gameImage;
    [SerializeField] private TMP_Text gameTitle;
    [SerializeField] private TMP_Text gameDescription;

    [Header("UI Panel")]
    [SerializeField] private GameObject modeSelectPanel;
    [SerializeField] private GameObject setAiPanel;
    [SerializeField] private GameObject inviteFriendPanel;
    [SerializeField] private GameObject roomSelectPanel;

    [Header("Game Description")]
    [SerializeField] private string xandoBrief;
    [SerializeField] private string archeryBrief;
    [SerializeField] private string fourinarowBrief;
    [SerializeField] private string minifootballBrief;
    [SerializeField] private string minigolfBrief;
    [SerializeField] private string dotsandboxesBrief;

    [Header("Game Images")]
    [SerializeField] private Image xandoImage;
    [SerializeField] private Image archeryImage;
    [SerializeField] private Image fourinarowImage;
    [SerializeField] private Image minifootballImage;
    [SerializeField] private Image minigolfImage;
    [SerializeField] private Image dotsandboxesImage;

    public void SelectGame(Wagr.GameName gameName)
    {
        switch (gameName)
        {
            case Wagr.GameName.xando:
                gameImage = xandoImage;
                gameDescription.text = xandoBrief;
                gameTitle.text = "X And O";
                break;
            case Wagr.GameName.dotsandboxes:
                gameImage = dotsandboxesImage;
                gameDescription.text = dotsandboxesBrief;
                gameTitle.text = "Dots And Boxes";
                break;
            case Wagr.GameName.fourinarow:
                gameImage = fourinarowImage;
                gameDescription.text = fourinarowBrief;
                gameTitle.text = "Four In A Row";
                break;
            case Wagr.GameName.minifootball:
                gameImage = minifootballImage;
                gameDescription.text = minifootballBrief;
                gameTitle.text = "Mini Football";
                break;
            case Wagr.GameName.archery:
                gameImage = archeryImage;
                gameDescription.text = archeryBrief;
                gameTitle.text = "Archery";
                break;
            case Wagr.GameName.minigolf:
                gameImage = minigolfImage;
                gameDescription.text = minigolfBrief;
                gameTitle.text = "Mini Golf";
                break;
        }
    }

    public void SelectMode(int mode)
    {
        modeSelectPanel.SetActive(false);

        switch (mode)
        {
            case 0: //Bot Mode
                setAiPanel.SetActive(true);
                break;
            case 1: //Invite Mode
                inviteFriendPanel.SetActive(true);
                break;
            case 2: //Room Mode
                roomSelectPanel.SetActive(true);
                break;
        }
    }


}
