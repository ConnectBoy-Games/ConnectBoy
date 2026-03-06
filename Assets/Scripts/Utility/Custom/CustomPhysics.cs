using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGolfPhysics
{
    // Configuration constants
    private const float DRAG = 0.98f;       // Friction (slower over time)
    private const float MIN_VELOCITY = 0.05f; // Threshold to stop the ball
    private const float BOUNCE_DAMPENING = 0.8f; // Energy lost on wall hit
    private const float BALL_RADIUS = 0.15f;

    // Inner class to track the ball during simulation
    public class SimBall
    {
        public Vector2 position;
        public Vector2 velocity;

        public SimBall(Vector2 startPos, Vector2 startVel)
        {
            position = startPos;
            velocity = startVel;
        }
    }

    /// <summary> Simulates an entire shot from start to finish. Returns the list of positions for the client to animate smoothly.</summary>
    public static List<Vector2> SimulateShot(Vector2 startPos, Vector2 forceVector, List<Collider> walls)
    {
        List<Vector2> trajectory = new List<Vector2>();
        SimBall ball = new SimBall(startPos, forceVector);

        // Safety break to prevent infinite loops
        int maxSteps = 1000;
        int steps = 0;

        // The Physics Loop
        while (ball.velocity.magnitude > MIN_VELOCITY && steps < maxSteps)
        {
            // 1. Move Ball
            ball.position += ball.velocity * Time.fixedDeltaTime;

            // 2. Check Collisions
            foreach (var wall in walls)
            {
                CheckWallCollision(ball, wall);
            }

            // 3. Apply Friction (Drag)
            ball.velocity *= DRAG;

            // 4. Record position for animation
            trajectory.Add(ball.position);
            steps++;
        }

        // Snap to zero if stopped
        if (ball.velocity.magnitude <= MIN_VELOCITY)
        {
            ball.velocity = Vector2.zero;
        }

        return trajectory;
    }

    /// <summary> Handles Circle-Line collision math manually. </summary>
    private static void CheckWallCollision(SimBall ball, Collider wall)
    {
        // Get the closest point on the line segment to the ball
        Vector2 closestPoint = wall.ClosestPointOnBounds(ball.position);

        // Calculate distance
        Vector2 diff = ball.position - closestPoint;
        float dist = diff.magnitude;

        // Collision detected
        if (dist < BALL_RADIUS)
        {
            // 1. Resolve Overlap (push ball out of wall)
            Vector2 normal = diff.normalized;
            float overlap = BALL_RADIUS - dist;
            ball.position += normal * overlap;

            // 2. Reflect Velocity (Bounce)
            // Formula: V_new = V_old - 2 * (V_old . Normal) * Normal
            ball.velocity = Vector2.Reflect(ball.velocity, normal);

            // 3. Apply energy loss (Bounce dampening)
            ball.velocity *= BOUNCE_DAMPENING;
        }
    }

    // Helper Math for line segments
    private static Vector2 GetClosestPointOnLineSegment(Vector2 start, Vector2 end, Vector2 point)
    {
        Vector2 line = end - start;
        float len = line.magnitude;
        line.Normalize();

        Vector2 v = point - start;
        float d = Vector2.Dot(v, line);

        // Clamp d between 0 and length of the line segment
        d = Mathf.Clamp(d, 0f, len);

        return start + line * d;
    }
}

public class CustomSoccerPhysics
{
    private const float FRICTION = 0.97f;
    private const float WALL_BOUNCE = 0.7f;
    private const float PIECE_BOUNCE = 0.8f; // Elasticity

    public static List<Dictionary<string, Vector2>> SimulateSoccerTurn(List<MiniBallEntity> allEntities, MiniBallPiece kickedPieceId, Vector2 kickForce, Collider[] walls)
    {
        // 1. Find the piece being kicked and apply initial force
        var target = allEntities.Find(e => e.Piece == kickedPieceId);
        if (target != null)
        {
            target.velX = kickForce.x;
            target.velY = kickForce.y;
        }

        List<Dictionary<string, Vector2>> frameHistory = new List<Dictionary<string, Vector2>>();
        int maxSteps = 1500;

        for (int step = 0; step < maxSteps; step++)
        {
            bool anythingMoving = false;
            Dictionary<string, Vector2> currentFrame = new Dictionary<string, Vector2>();

            // A. Resolve Collisions between all entities (Circles)
            for (int i = 0; i < allEntities.Count; i++)
            {
                for (int j = i + 1; j < allEntities.Count; j++)
                {
                    ResolveCircleCollision(allEntities[i], allEntities[j]);
                }
            }

            // B. Update Positions and check Walls
            foreach (var entity in allEntities)
            {
                Vector2 pos = new Vector2(entity.PosX, entity.PosZ);
                Vector2 vel = new Vector2(entity.velX, entity.velY);

                if (vel.magnitude > 0.01f)
                {
                    anythingMoving = true;
                    pos += vel * Time.fixedDeltaTime;
                    vel *= FRICTION;

                    // Wall Collision (Simplified Box check or use the Golf Line logic)
                    if (pos.x < -10 || pos.x > 10) vel.x *= -WALL_BOUNCE;
                    if (pos.y < -5 || pos.y > 5) vel.y *= -WALL_BOUNCE;
                }
                else
                {
                    vel = Vector2.zero;
                }

                entity.PosX = pos.x; entity.PosZ = pos.y;
                entity.velX = vel.x; entity.velY = vel.y;

                currentFrame.Add(entity.Piece.ToString(), pos);
            }

            frameHistory.Add(currentFrame);
            if (!anythingMoving) break;
        }

        return frameHistory;
    }

    private static void ResolveCircleCollision(MiniBallEntity a, MiniBallEntity b)
    {
        Vector2 posA = new Vector2(a.PosX, a.PosZ);
        Vector2 posB = new Vector2(b.PosX, b.PosZ);
        float distance = Vector2.Distance(posA, posB);
        float minDistance = MiniBallEntity.radius + MiniBallEntity.radius;

        if (distance < minDistance)
        {
            Vector2 normal = (posA - posB).normalized;

            // 1. Separate the overlapping circles
            float overlap = minDistance - distance;
            Vector2 separation = normal * (overlap / 2f);
            a.PosX += separation.x; a.PosZ += separation.y;
            b.PosX -= separation.x; b.PosZ -= separation.y;

            // 2. Simple Elastic Collision (Momentum Swap)
            Vector2 velA = new Vector2(a.velX, a.velY);
            Vector2 velB = new Vector2(b.velX, b.velY);

            float relativeVelocity = Vector2.Dot(velA - velB, normal);
            if (relativeVelocity > 0) return; // Already moving apart

            float impulse = (2f * relativeVelocity) / (MiniBallEntity.mass * 2);
            Vector2 finalImpulse = impulse * normal * PIECE_BOUNCE;

            a.velX -= finalImpulse.x * MiniBallEntity.mass;
            a.velY -= finalImpulse.y * MiniBallEntity.mass;
            b.velX += finalImpulse.x * MiniBallEntity.mass;
            b.velY += finalImpulse.y * MiniBallEntity.mass;
        }
    }

    private IEnumerator AnimateSoccerTurn(List<Dictionary<string, Vector2>> trajectory)
    {
        foreach (var frame in trajectory)
        {
            foreach (var entry in frame)
            {
                // Find the GameObject in your scene that matches the ID (e.g., "Ball")
                GameObject visualPiece = GameObject.Find(entry.Key);
                if (visualPiece != null)
                {
                    visualPiece.transform.position = entry.Value;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}

