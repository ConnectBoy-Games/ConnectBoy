using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameDetailPanel : MonoBehaviour
{
    public UnityAction backAction;

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

    public void GoBack()
    {
        if(modeSelectPanel.activeInHierarchy)
        {
            backAction();
            gameObject.SetActive(false); //Disable itself
        }
        else
        {
            modeSelectPanel.SetActive(true);
            setAiPanel.SetActive(false);
            inviteFriendPanel.SetActive(false);
            roomSelectPanel.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            backAction();
        }
    }

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
                GameManager.gameSession.gameName = Wagr.GameName.xando;
                break;
            case Wagr.GameName.dotsandboxes:
                gameImage.sprite = dotsandboxesImage;
                backgroundImage.color = dotsandboxesColor;
                gameDescription.text = dotsandboxesBrief;
                gameTitle.text = "Dots And Boxes";
                GameManager.gameSession.gameName = Wagr.GameName.dotsandboxes;
                break;
            case Wagr.GameName.fourinarow:
                gameImage.sprite = fourinarowImage;
                backgroundImage.color = fourinarowColor;
                gameDescription.text = fourinarowBrief;
                gameTitle.text = "Four In A Row";
                GameManager.gameSession.gameName = Wagr.GameName.fourinarow;
                break;
            case Wagr.GameName.minisoccer:
                gameImage.sprite = minisoccerImage;
                backgroundImage.color = minisoccerColor;
                gameDescription.text = minisoccerBrief;
                gameTitle.text = "Mini Soccer";
                GameManager.gameSession.gameName = Wagr.GameName.minisoccer;
                break;
            case Wagr.GameName.archery:
                gameImage.sprite = archeryImage;
                backgroundImage.color = archeryColor;
                gameDescription.text = archeryBrief;
                gameTitle.text = "Archery";
                GameManager.gameSession.gameName = Wagr.GameName.archery;
                break;
            case Wagr.GameName.minigolf:
                gameImage.sprite = minigolfImage;
                backgroundImage.color = minigolfColor;
                gameDescription.text = minigolfBrief;
                gameTitle.text = "Mini Golf";
                GameManager.gameSession.gameName = Wagr.GameName.minigolf;
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
                GameManager.gameSession.gameMode = GameMode.vsBot;
                break;
            case 1: //Invite Mode
                inviteFriendPanel.SetActive(true);
                GameManager.gameSession.gameMode = GameMode.vsPlayer;
                break;
            case 2: //Room Mode
                roomSelectPanel.SetActive(true);
                GameManager.gameSession.gameMode = GameMode.vsPlayer;
                break;
        }
    }
}
