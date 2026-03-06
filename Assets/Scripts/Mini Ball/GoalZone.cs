using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public TeamManager teamManager;

    public bool Contains(Vector3 point)
    {
        return GetComponent<Collider>().bounds.Contains(point);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ball")) //If the ball entered the post
        {
            teamManager.scoreCount++;
        }
    }
}
