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
            GameManager.instance.GetComponent<AudioManager>().PlayVictorySound(); //Play the goal sound effect
            other.GetComponent<Ball>().PlayEffect(); //Play the effect of the ball entering the post
            teamManager.scoreCount++;
        }
    }
}
