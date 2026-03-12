using UnityEngine;

public class Ball : MonoBehaviour
{
    public static PieceState ballState;
    public MiniBallPiece piece = MiniBallPiece.Ball;
    public bool moved = false; //Used to check if the ball moved in a particular turn

    [SerializeField] MiniBallManager manager;
    [SerializeField] ParticleSystem starEffect; //The particle effect that plays when a goal is scored
    [SerializeField] float minVelocity = 0.1f; //The minimum velocity the ball needs to be considered moving
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (ballState == PieceState.Moving && rb.linearVelocity.magnitude <= minVelocity)
        {
            //Stop the movement of the ball //Record the move //Switch turns
            ballState = PieceState.Idle;
            rb.linearVelocity = Vector3.zero;
            manager.BallMoved();
            moved = true;
        }
    }

    public void PlayEffect()
    {
        starEffect.Play();
    }

    public void ResetBall()
    {
        moved = false;
        starEffect.Stop();
        rb.linearVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb.linearVelocity.magnitude > minVelocity)
        {
            ballState = PieceState.Moving;
        }
    }

    public enum PieceState : byte
    {
        Idle = 0, //Not Moving
        Moving = 1, //Moving
    }
}
