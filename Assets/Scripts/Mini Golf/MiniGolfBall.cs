using UnityEngine;

public class MiniGolfBall : MonoBehaviour
{
    public MiniGolfManager manager;
    public bool isPlayable = false; //Used to limit whose player pieces can be played(Turn management)
    public bool locked = false; //Locks up all the player pieces

    [Header("Shot Trajectory")]
    [SerializeField] LineRenderer aim;
    Ball.PieceState pieceState = Ball.PieceState.Idle; //Is the ball moving or not
    public int resolution = 30; // Number of dots
    public float timeStep = 0.1f; // Seconds between dots

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
        if (isPlayable && !locked) //The ball is  active and the game state is not locked
        {
            isDragging = true;
            aim.gameObject.SetActive(true);
            startPosition = transform.position;
        }
    }

    void OnMouseUp()
    {
        var mag = (startPosition - currentPosition).magnitude;

        //Minimum magnitude is 40
        if (mag > 0.1f && isPlayable && !locked) //The Player's pieces are currently active and the game state is not locked
        {
            isDragging = false;
            MakeMove(startPosition - currentPosition);
        }
        aim.gameObject.SetActive(false);
    }

    public void MakeMove(Vector3 direction)
    {
        locked = true; //Lock all the pieces so none can make a move
        var mag = 3 * direction.magnitude;
        rb.AddForce(mag * direction.normalized, ForceMode.VelocityChange);
        manager.MakeMove();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            GameManager.instance.GetComponent<AudioManager>().PlayPlaceSound();
        }
        else if (collision.collider.CompareTag("Hole"))
        {
            GameManager.instance.GetComponent<AudioManager>().PlayVictorySound();
        }
    }

    public void HideTrajectory()
    {
        aim.positionCount = 0;
    }

    public void ShowTrajectory(Vector2 startPos, Vector2 force)
    {
        Vector2 velocity = force / 1.0f; // Simplified: Initial velocity = Force / Mass
        aim.positionCount = resolution;
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;

            // Formula: s = ut + 0.5at^2
            // x = v0x * t
            // y = v0y * t + 0.5 * gravity * t^2
            float x = velocity.x * t;
            float y = (velocity.y * t) + (0.5f * Physics2D.gravity.y * t * t);

            points[i] = new Vector3(startPos.x + x, startPos.y + y, 0);

            // Optional: Stop drawing if the line hits a wall
            if (i > 0)
            {
                RaycastHit2D hit = Physics2D.Linecast(points[i - 1], points[i]);
                if (hit.collider != null)
                {
                    aim.positionCount = i;
                    break;
                }
            }
        }
        aim.SetPositions(points);
    }
}


