using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    public MiniBallPiece piece;
    public TeamManager teamManager;
    public bool isPlayable = false; //Used to limit whose player pieces can be played(Turn management)
    public static bool locked = false; //Locks up all the player pieces

    [Header("UI")]
    [SerializeField] LineRenderer aim;
    [SerializeField] GameObject indicator;
    [SerializeField] Ball.PieceState pieceState = Ball.PieceState.Idle; //Is the player moving or not

    //Physics Handling
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 startPosition, currentPosition;
    private float minVelocity = 0.1f; //The minimum velocity the ball needs to be considered moving

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDragging)
        {
            currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 pos = new Vector3((startPosition - currentPosition).normalized.x, 0, (startPosition - currentPosition).normalized.z);
            aim.SetPosition(aim.positionCount - 1, pos * 15);
        }
        else if (pieceState == Ball.PieceState.Idle && rb.linearVelocity.magnitude >= minVelocity)
        {
            pieceState = Ball.PieceState.Moving;
        }
        else if (pieceState == Ball.PieceState.Moving && rb.linearVelocity.magnitude <= minVelocity)
        {
            //Stop the player from moving
            rb.linearVelocity = Vector2.zero;
            pieceState = Ball.PieceState.Idle;
        }
    }

    void OnMouseDown()
    {
        if (isPlayable && !locked) //The Player's pieces are currently active and the game state is not locked
        {
            isDragging = true;
            aim.gameObject.SetActive(true);
            startPosition = transform.position;
            teamManager.SetIndicator(false); //Disable the indicator for all the players in the team
        }
    }

    void OnMouseUp()
    {
        var mag = (startPosition - currentPosition).magnitude;

        //Minimum magnitude is 40
        if (mag > 40 && isPlayable && !locked) //The Player's pieces are currently active and the game state is not locked
        {
            isDragging = false;
            MakeMove(startPosition - currentPosition);
        }
        else if (isPlayable) //It is the player's turn
        {
            teamManager.SetIndicator(true);
        }

        aim.gameObject.SetActive(false);
    }

    public void MakeMove(Vector3 direction)
    {
        locked = true; //Lock all the pieces so none can make a move
        var mag = 3 * direction.magnitude;
        rb.AddForce(mag * direction.normalized, ForceMode.VelocityChange);
        teamManager.MakeMove(); //Report that a move has been made
    }

    public void SetIndictator(bool state)
    {
        indicator.SetActive(state);
    }

    void OnCollisionEnter(Collision collision)
    {
        GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();
    }
}
