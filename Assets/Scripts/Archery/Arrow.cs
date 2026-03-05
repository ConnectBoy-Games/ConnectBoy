using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 30;

    [Header("Wind Influence")]
    [Tooltip("How much the wind deflects the arrow while in flight. " +
             "Heavier arrows resist wind more – lower this value for realism.")]
    [SerializeField] private float windFlightInfluence = 0.04f;

    private Vector3 target;
    private ArcheryControl.ArrowState state; //State of the bow that fired the arrow

    private void Update()
    {
        if (state == ArcheryControl.ArrowState.Released)
        {
            // ---- Wind drift ----
            // Shift the effective target slightly each frame based on current wind.
            // This makes the arrow curve mid-flight during strong gusts.
            ApplyWindDrift();

            // ---- Move the arrow towards the (possibly wind-shifted) target ----
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            // ---- Rotate the arrow to face the target ----
            Vector3 targetDirection = target - transform.position;
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
            }

            // ---- Arrived ----
            if (Vector3.Distance(transform.position, target) < 0.05f)
            {
                state = ArcheryControl.ArrowState.Hit;
            }
        }
    }

    /// <summary>
    /// Shifts the arrow's target point by a small wind-driven offset each frame.
    /// The effect is stronger when the arrow is still far from the target
    /// (longer remaining flight time = more time for wind to push it).
    /// </summary>
    private void ApplyWindDrift()
    {
        if (WindSystem.Instance == null) return;

        Vector2 wind             = WindSystem.Instance.CurrentWind;
        float   distanceLeft     = Vector3.Distance(transform.position, target);

        // Scale influence by remaining distance so drift tapers off near the board
        float   distanceFactor   = Mathf.Clamp01(distanceLeft / 10f); // 10 = reference range
        Vector3 drift            = new Vector3(wind.x, wind.y, 0f)
                                   * windFlightInfluence
                                   * distanceFactor
                                   * Time.deltaTime;

        target += drift;
    }

    public void SetTarget(Vector3 targetPosition, ref ArcheryControl.ArrowState arrowState)
    {
        target = targetPosition;
        state  = arrowState;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy the arrow if it reaches the target
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
