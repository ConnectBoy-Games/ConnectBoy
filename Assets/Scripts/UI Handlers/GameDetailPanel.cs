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
    [SerializeField] private Image backgroundImage;
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
    [SerializeField] private string minisoccerBrief;
    [SerializeField] private string minigolfBrief;
    [SerializeField] private string dotsandboxesBrief;

    [Header("Game Images")]
    [SerializeField] private Sprite xandoImage;
    [SerializeField] private Sprite archeryImage;
    [SerializeField] private Sprite fourinarowImage;
    [SerializeField] private Sprite minisoccerImage;
    [SerializeField] private Sprite minigolfImage;
    [SerializeField] private Sprite dotsandboxesImage;

    [Header("Game Color")]
    [SerializeField] private Color xandoColor;
    [SerializeField] private Color archeryColor;
    [SerializeField] private Color fourinarowColor;
    [SerializeField] private Color minisoccerColor;
    [SerializeField] private Color minigolfColor;
    [SerializeField] private Color dotsandboxesColor;

    void OnEnable()
    {
        modeSelectPanel.SetActive(true);
        setAiPanel.SetActive(false);
        inviteFriendPanel.SetActive(false);
        roomSelectPanel.SetActive(false);
    }

    public void SelectGame(Wagr.GameName gameName)
    {
        switch (gameName)
        {
            case Wagr.GameName.xando:
                gameImage.sprite = xandoImage;
                backgroundImage.color = xandoColor;
                gameDescription.text = xandoBrief;
                gameTitle.text = "X And O";
                break;
            case Wagr.GameName.dotsandboxes:
                gameImage.sprite = dotsandboxesImage;
                backgroundImage.color = dotsandboxesColor;
                gameDescription.text = dotsandboxesBrief;
                gameTitle.text = "Dots And Boxes";
                break;
            case Wagr.GameName.fourinarow:
                gameImage.sprite = fourinarowImage;
                backgroundImage.color = fourinarowColor;
                gameDescription.text = fourinarowBrief;
                gameTitle.text = "Four In A Row";
                break;
            case Wagr.GameName.minisoccer:
                gameImage.sprite = minisoccerImage;
                backgroundImage.color = minisoccerColor;
                gameDescription.text = minisoccerBrief;
                gameTitle.text = "Mini Soccer";
                break;
            case Wagr.GameName.archery:
                gameImage.sprite = archeryImage;
                backgroundImage.color = archeryColor;
                gameDescription.text = archeryBrief;
                gameTitle.text = "Archery";
                break;
            case Wagr.GameName.minigolf:
                gameImage.sprite = minigolfImage;
                backgroundImage.color = minigolfColor;
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
