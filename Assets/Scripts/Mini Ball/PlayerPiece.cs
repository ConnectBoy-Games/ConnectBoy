using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    [SerializeField] TeamManager teamManager;
    public MiniBallPiece piece;

    [Header("UI")]
    public static bool locked = false; //Locks up all the player pieces
    public bool isPlayable = false; //Used to limit whose player pieces can be played

    [Header("UI")]
    [SerializeField] private GameObject indicator;

    private bool isDragging = false;
    private Vector3 startPosition, currentPosition;

    void Update()
    {
        if (isDragging)
        {
            currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void OnMouseDown()
    {
        if(isPlayable && !locked) //The Player's pieces are currently active and the game state is not locked
        {
            isDragging = true;
            startPosition = transform.position;
            SetIndictator(false);
        }
    }

    void OnMouseUp()
    {
        //Minimum magnitude is 40
        if (isPlayable && !locked) //The Player's pieces are currently active and the game state is not locked
        {
            //locked = true; //Lock all the pieces so none can make a move

            var rb = GetComponent<Rigidbody>();
            var mag = (startPosition - currentPosition).magnitude;
            var magnitude = 3 * mag;
            var direction = (startPosition - currentPosition).normalized;

            Debug.Log($"Magnitude: {mag}, Direction: {direction}");
            rb.AddForce(magnitude * direction, ForceMode.VelocityChange);

            MiniBallMove move = new MiniBallMove
            {
                forceX = (magnitude * direction).x,
                forceY = (magnitude * direction).y,
                PieceId = piece
            };
        }
    }

    public void SetIndictator(bool state)
    {
        indicator.SetActive(state);
    }
}
