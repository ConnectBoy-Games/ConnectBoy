using UnityEngine;

public class MiniGolfBall : MonoBehaviour
{
    [SerializeField] private MiniGolfManager manager;

    [Header("Shot Trajectory")]
    public LineRenderer lineRenderer;
    public int resolution = 30; // Number of dots
    public float timeStep = 0.1f; // Seconds between dots

    private bool isDragging = false;
    private Vector3 startPosition;
    private Vector3 currentPosition;

    void Update()
    {
        if (isDragging)
        {
            currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseDown()
    {
        isDragging = true;
        startPosition = transform.position;
    }

    private void OnMouseUp()
    {
        var rb = GetComponent<Rigidbody>();

        var magnitude = -2 * (currentPosition - startPosition).magnitude;
        var direction = (currentPosition - startPosition).normalized;
        rb.AddForce(magnitude * direction, ForceMode.VelocityChange);

        //manager.OnLocalPlayerShoot(magnitude * direction);
    }

    public void HideTrajectory()
    {
        lineRenderer.positionCount = 0;
    }

    public void ShowTrajectory(Vector2 startPos, Vector2 force)
    {
        Vector2 velocity = force / 1.0f; // Simplified: Initial velocity = Force / Mass
        lineRenderer.positionCount = resolution;
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
                    lineRenderer.positionCount = i;
                    break;
                }
            }
        }
        lineRenderer.SetPositions(points);
    }
}