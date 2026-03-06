using System.Collections.Generic;
using UnityEngine;

public class MiniGolfMap : MonoBehaviour
{
    public Collider hole;
    public List<Collider> walls;
    public List<Collider> obstacles;

    public bool CheckBall(Vector3 ballPosition)
    {
        float distToHole = Vector2.Distance(ballPosition, hole.transform.position);

        if (hole.bounds.Contains(ballPosition) || distToHole < 0.3f) // Threshold for sinking the ball
        {
            return true;
        }

        return false;
    }
}
