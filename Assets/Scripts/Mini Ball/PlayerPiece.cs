using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    [SerializeField] TeamManager teamManager;
    [SerializeField] LineRenderer aim;
    public MiniBallPiece piece;

    [Header("UI")]
    public static bool locked = false; //Locks up all the player pieces
    public bool isPlayable = false; //Used to limit whose player pieces can be played

    [Header("UI")]
    [SerializeField] private GameObject indicator;

    private Rigidbody rb; 
    private bool isDragging = false;
    private Vector3 startPosition, currentPosition;

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
    }

    void OnMouseDown()
    {
        if(isPlayable && !locked) //The Player's pieces are currently active and the game state is not locked
        {
            isDragging = true;
            aim.gameObject.SetActive(true);
            startPosition = transform.position;
            SetIndictator(false);
        }
    }

    void OnMouseUp()
    {
        var mag = (startPosition - currentPosition).magnitude;
        
        //Minimum magnitude is 40
        if (mag > 40 && isPlayable && !locked) //The Player's pieces are currently active and the game state is not locked
        {
            //locked = true; //Lock all the pieces so none can make a move
            Ball.ballState = Ball.BallState.Moving;
            
            var magnitude = 3 * mag;
            var direction = (startPosition - currentPosition).normalized;
            rb.AddForce(magnitude * direction, ForceMode.VelocityChange);
        }

        aim.gameObject.SetActive(false);
    }

    public void SetIndictator(bool state)
    {
        indicator.SetActive(state);
    }
}
