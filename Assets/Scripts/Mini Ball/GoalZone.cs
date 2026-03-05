using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public bool Contains(Vector3 point)
    {
        return GetComponent<Collider>().bounds.Contains(point);
    }
}
