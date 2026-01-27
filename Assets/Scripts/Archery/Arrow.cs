using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 30; 
    private Vector3 target;
    private ArcheryControl.ArrowState state; //State of the bow that fired the arrow

    private void Update()
    {
        if (state == ArcheryControl.ArrowState.Released)
        {
            // Move the arrow towards the target
            float step = speed * Time.deltaTime; // Arrow speed
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            // Rotate the arrow to face the target
            Vector3 targetDirection = target - transform.position;
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
            }
        }
    }

    public void SetTarget(Vector3 targetPosition, ref ArcheryControl.ArrowState arrowState)
    {
        target = targetPosition;
        state = arrowState;
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
