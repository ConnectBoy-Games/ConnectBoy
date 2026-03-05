using UnityEngine;

public class Ball : MonoBehaviour
{
    public static BallState ballState;
    public MiniBallPiece piece = MiniBallPiece.Ball;

    [SerializeField] MiniBallManager manager;
    [SerializeField] ParticleSystem starEffect; //The particle effect that plays when a goal is scored
    [SerializeField] float minVelocity = 0.1f; //The minimum velocity the ball needs to be considered moving
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (ballState == BallState.Moving && rb.velocity.magnitude <= minVelocity)
        {
            Debug.Log("Ball Stopped Moving!");
            ballState = BallState.Idle;

            //Stop the movement of the ball //Record the move //Switch turns
            rb.velocity = Vector3.zero;

            manager.MadeMove(new MiniBallMove
            {
                forceX = 0,
                forceY = 0,
                PieceId = piece
            });
        }
    }

    public enum BallState : byte
    {
        Idle = 0, //Moving
        Moving = 1, //Not Moving
    }
}
