using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameDetailPanel : MonoBehaviour
{
    public UnityAction backAction;

    [Header("Game Detail UI")]
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
    [SerializeField] private Image xandoImage;
    [SerializeField] private Image archeryImage;
    [SerializeField] private Image fourinarowImage;
    [SerializeField] private Image minisoccerImage;
    [SerializeField] private Image minigolfImage;
    [SerializeField] private Image dotsandboxesImage;

    [Header("Game Color")]
    [SerializeField] private Color xandoColor;
    [SerializeField] private Color archeryColor;
    [SerializeField] private Color fourinarowColor;
    [SerializeField] private Color minisoccerColor;
    [SerializeField] private Color minigolfColor;
    [SerializeField] private Color dotsandboxesColor;

    [Header("Mode Buttons")]
    [SerializeField] private Button friendButton;
    [SerializeField] private Button roomButton;

    public void GoBack()
    {
        if (modeSelectPanel.activeInHierarchy)
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
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            backAction();
        }
    }

    void OnEnable()
    {
        setAiPanel.SetActive(false);
        inviteFriendPanel.SetActive(false);
        roomSelectPanel.SetActive(false);

        modeSelectPanel.SetActive(true);
        //Enable/Disable mode buttons based on login state
        //friendButton.interactable = GameManager.instance.accountManager.loginState == LoginState.loggedIn;
        roomButton.interactable = GameManager.instance.accountManager.loginState == LoginState.loggedIn;
    }

    public void SelectGame(Wagr.GameName gameName)
    {
        ClearImages(); //Reset all game images before showing the selected one
        GameManager.gameSession = new Wagr.Session(gameName); //Create/Reset game session (A game vs a bot)
        switch (gameName)
        {
            case Wagr.GameName.xando:
                xandoImage.gameObject.SetActive(true);
                backgroundImage.color = xandoColor;
                gameDescription.text = xandoBrief;
                gameTitle.text = "X And O";
                break;
            case Wagr.GameName.dotsandboxes:
                dotsandboxesImage.gameObject.SetActive(true);
                backgroundImage.color = dotsandboxesColor;
                gameDescription.text = dotsandboxesBrief;
                gameTitle.text = "Dots And Boxes";
                break;
            case Wagr.GameName.fourinarow:
                fourinarowImage.gameObject.SetActive(true);
                backgroundImage.color = fourinarowColor;
                gameDescription.text = fourinarowBrief;
                gameTitle.text = "Four In A Row";
                break;
            case Wagr.GameName.minisoccer:
                minisoccerImage.gameObject.SetActive(true);
                backgroundImage.color = minisoccerColor;
                gameDescription.text = minisoccerBrief;
                gameTitle.text = "Mini Soccer";
                break;
            case Wagr.GameName.archery:
                archeryImage.gameObject.SetActive(true);
                backgroundImage.color = archeryColor;
                gameDescription.text = archeryBrief;
                gameTitle.text = "Archery";
                break;
            case Wagr.GameName.minigolf:
                minigolfImage.gameObject.SetActive(true);
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

    private void ClearImages()
    {
        //Reset all game images 
        xandoImage.gameObject.SetActive(false);
        archeryImage.gameObject.SetActive(false);
        fourinarowImage.gameObject.SetActive(false);
        minisoccerImage.gameObject.SetActive(false);
        minigolfImage.gameObject.SetActive(false);
        dotsandboxesImage.gameObject.SetActive(false);
    }
}
