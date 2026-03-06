using UnityEngine;

public class Ball : MonoBehaviour
{
    public static PieceState ballState;
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
        if (ballState == PieceState.Moving && rb.velocity.magnitude <= minVelocity)
        {
            //Stop the movement of the ball //Record the move //Switch turns
            ballState = PieceState.Idle;
            rb.velocity = Vector3.zero;
            manager.BallMoved();
        }
    }

    public void PlayEffect()
    {
        starEffect.Play();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > minVelocity)
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
