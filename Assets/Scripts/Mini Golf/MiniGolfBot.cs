using System.Collections.Generic;
using UnityEngine;

public class MiniGolfBot
{
    private BotDifficulty difficulty;
    private Vector2 holePosition;
    private List<WallData> walls;
    private const float MAX_FORCE = 15f;
    private const float MIN_FORCE = 3f;
    private const float HOLE_RADIUS = 0.3f;

    public MiniGolfBot(BotDifficulty difficulty, Vector2 holePosition, List<WallData> walls)
    {
        this.difficulty = difficulty;
        this.holePosition = holePosition;
        this.walls = walls;
    }

    public void UpdateLevelData(Vector2 holePos, List<WallData> currentWalls)
    {
        this.holePosition = holePos;
        this.walls = currentWalls;
    }

    public Vector2 ThinkMove(Vector2 ballPos)
    {
        return difficulty switch
        {
            BotDifficulty.low => CalculateEasyMove(ballPos),
            BotDifficulty.medium => CalculateMediumMove(ballPos),
            BotDifficulty.high => CalculateHardMove(ballPos),
            _ => CalculateEasyMove(ballPos)
        };
    }

    private Vector2 CalculateEasyMove(Vector2 ballPos)
    {
        // Easy: Shoots roughly towards the hole with significant noise
        Vector2 direction = (holePosition - ballPos).normalized;
        float dist = Vector2.Distance(ballPos, holePosition);
        
        // Add random angle (+- 25 degrees)
        float randomAngle = Random.Range(-25f, 25f);
        direction = RotateVector(direction, randomAngle);

        // Randomize force roughly based on distance
        float force = Mathf.Clamp(dist * 0.8f + Random.Range(-3f, 3f), MIN_FORCE, MAX_FORCE);
        
        return direction * force;
    }

    private Vector2 CalculateMediumMove(Vector2 ballPos)
    {
        // Medium: Shoots more accurately towards the hole, but still no bounce consideration
        Vector2 direction = (holePosition - ballPos).normalized;
        float dist = Vector2.Distance(ballPos, holePosition);

        // Add small random angle (+- 5 degrees)
        float randomAngle = Random.Range(-5f, 5f);
        direction = RotateVector(direction, randomAngle);

        // More accurate force
        float force = Mathf.Clamp(dist * 1.1f + Random.Range(-1f, 1f), MIN_FORCE, MAX_FORCE);

        return direction * force;
    }

    private Vector2 CalculateHardMove(Vector2 ballPos)
    {
        // Hard: Simulates many shots including bank shots to find the best path
        Vector2 bestMove = Vector2.zero;
        float closestDist = float.MaxValue;

        // Search through 72 directions (every 5 degrees)
        for (int i = 0; i < 72; i++)
        {
            float angle = i * 5f;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Try 3 different force levels
            float[] forceLevels = { MAX_FORCE * 0.5f, MAX_FORCE * 0.75f, MAX_FORCE };
            foreach (float force in forceLevels)
            {
                Vector2 testMove = dir * force;
                var trajectory = CustomGolfPhysics.SimulateShot(ballPos, testMove, walls);

                // Find the closest this trajectory gets to the hole
                foreach (Vector2 point in trajectory)
                {
                    float d = Vector2.Distance(point, holePosition);
                    if (d < closestDist)
                    {
                        closestDist = d;
                        bestMove = testMove;
                    }

                    // If we found a shot that goes IN, we can stop or pick it
                    if (d < HOLE_RADIUS * 0.5f) return testMove;
                }
            }
        }

        return bestMove;
    }

    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }
}
