using UnityEngine;

/// <summary>
/// MiniBallBot selects which team-2 player piece to kick and in what direction,
/// using Unity's physics engine to resolve the actual collision and ball movement.
///
/// Core idea: when a player is pushed toward the ball the ball travels away from
/// that player.  So the approximate post-hit ball direction is:
///     hitDir = (ballPos – playerPos).normalized
/// The bot scores each player by how well that direction aligns with the vector
/// from the ball to the opponent's post (team1.goalZone), then applies a force
/// along that direction so the player collides with the ball and drives it on target.
/// </summary>
public class MiniBallBot
{
    private readonly BotDifficulty difficulty;

    private const float MIN_FORCE = 45f;
    private const float MAX_FORCE = 65f;
    private const float GOAL_DANGER_RADIUS = 2f;  // triggers defensive clear (hard only)

    public MiniBallBot(BotDifficulty difficulty)
    {
        this.difficulty = difficulty;
    }

    public MiniBallMove MakeMove(TeamManager team1, TeamManager team2, Transform ball)
    {
        return difficulty switch
        {
            BotDifficulty.low => ThinkEasy(team1, team2, ball),
            BotDifficulty.medium => ThinkMedium(team1, team2, ball),
            BotDifficulty.high => ThinkHard(team1, team2, ball),
            _ => ThinkEasy(team1, team2, ball),
        };
    }

    // ── Easy ──────────────────────────────────────────────────────────────────
    // Scans all players. For each, computes the natural hit direction (player→ball
    // extended) and scores it against the ideal aim vector.
    // Adds significant random angular noise so the ball is imprecise.
    private MiniBallMove ThinkEasy(TeamManager team1, TeamManager team2, Transform ball)
    {
        Vector2 ballPos = Flat(ball.position);
        Vector2 oppGoalPos = Flat(team1.goalZone.transform.position);
        Vector2 idealDir = (oppGoalPos - ballPos).normalized;

        MiniBallMove bestMove = null;
        float bestScore = float.MinValue;

        foreach (MiniBallEntity player in team2.GetPlayers())
        {
            Vector2 playerPos = new(player.PosX, player.PosZ);

            // Natural ball-travel direction: player punches through the ball
            Vector2 hitDir = (ballPos - playerPos).normalized;

            // Add heavy noise so easy mode is sloppy
            float noiseAngle = Random.Range(-35f, 35f);
            Vector2 noisyDir = RotateVector(hitDir, noiseAngle);

            float score = Vector2.Dot(noisyDir, idealDir);
            if (score > bestScore)
            {
                bestScore = score;
                float force = Random.Range(MIN_FORCE * 0.5f, MAX_FORCE * 0.55f);
                // Force is applied to the player in the direction of the ball
                bestMove = BuildMove(player.Piece, hitDir * force);
            }
        }
        return bestMove;
    }

    // ── Medium ────────────────────────────────────────────────────────────────
    // Scans all players. Also samples a small arc of approach angles around each
    // player's natural hit direction so it can aim more deliberately.
    // Penalises hitting the ball toward own goal.
    private MiniBallMove ThinkMedium(TeamManager team1, TeamManager team2, Transform ball)
    {
        Vector2 ballPos = Flat(ball.position);
        Vector2 oppGoalPos = Flat(team1.goalZone.transform.position);
        Vector2 ownGoalPos = Flat(team2.goalZone.transform.position);
        Vector2 idealDir = (oppGoalPos - ballPos).normalized;
        Vector2 dangerDir = (ownGoalPos - ballPos).normalized;

        MiniBallMove bestMove = null;
        float bestScore = float.MinValue;

        foreach (MiniBallEntity player in team2.GetPlayers())
        {
            Vector2 playerPos = new(player.PosX, player.PosZ);
            Vector2 baseHitDir = (ballPos - playerPos).normalized;

            // Sample 5 angles around the natural hit line (±20°, ±10°, 0°)
            float[] offsets = { -20f, -10f, 0f, 10f, 20f };
            foreach (float offset in offsets)
            {
                Vector2 testHitDir = RotateVector(baseHitDir, offset);

                float goalAlign = Vector2.Dot(testHitDir, idealDir);
                float dangerAlign = Vector2.Dot(testHitDir, dangerDir);
                float score = goalAlign - Mathf.Max(0f, dangerAlign) * 1.5f;

                if (score > bestScore)
                {
                    bestScore = score;
                    // Push the player from the opposite side of the intended ball exit
                    // (approach from behind the ball relative to the adjusted hit direction)
                    Vector2 approachDir = RotateVector(baseHitDir, offset);
                    float force = MAX_FORCE * 0.75f;
                    bestMove = BuildMove(player.Piece, approachDir * force);
                }
            }
        }
        return bestMove;
    }

    // ── Hard ──────────────────────────────────────────────────────────────────
    // Defensive clear if the ball is near own goal.
    // Offensive: fine-grained arc scan (every 5°, ±30°) per player.
    // Strong own-goal penalty; always uses maximum force.
    private MiniBallMove ThinkHard(TeamManager team1, TeamManager team2, Transform ball)
    {
        Vector2 ballPos = Flat(ball.position);
        Vector2 ownGoalPos = Flat(team2.goalZone.transform.position);

        // DEFENSIVE: if ball is dangerously close to own goal, clear it away
        if (Vector2.Distance(ballPos, ownGoalPos) < GOAL_DANGER_RADIUS)
        {
            MiniBallMove clearMove = TryClearBall(team1, team2, ball);
            if (clearMove != null) return clearMove;
        }

        // OFFENSIVE
        Vector2 oppGoalPos = Flat(team1.goalZone.transform.position);
        Vector2 idealDir = (oppGoalPos - ballPos).normalized;
        Vector2 dangerDir = (ownGoalPos - ballPos).normalized;

        MiniBallMove bestMove = null;
        float bestScore = float.MinValue;

        foreach (MiniBallEntity player in team2.GetPlayers())
        {
            Vector2 playerPos = new(player.PosX, player.PosZ);
            Vector2 baseHitDir = (ballPos - playerPos).normalized;

            // Fine arc: every 5° from –30° to +30°
            for (int deg = -30; deg <= 30; deg += 5)
            {
                Vector2 testHitDir = RotateVector(baseHitDir, deg);

                float goalAlign = Vector2.Dot(testHitDir, idealDir);
                float dangerAlign = Vector2.Dot(testHitDir, dangerDir);
                float score = goalAlign - Mathf.Max(0f, dangerAlign) * 3f;

                if (score > bestScore)
                {
                    bestScore = score;
                    Vector2 approachDir = RotateVector(baseHitDir, deg);
                    bestMove = BuildMove(player.Piece, approachDir * MAX_FORCE);
                }
            }
        }
        return bestMove;
    }

    // ── Defensive clear ───────────────────────────────────────────────────────
    // The bot player closest to the ball kicks it straight toward the opponent's post.
    private MiniBallMove TryClearBall(TeamManager team1, TeamManager team2, Transform ball)
    {
        Vector2 ballPos = Flat(ball.position);
        MiniBallPiece bestPiece = MiniBallPiece.Player2_Piece1;
        float closestDist = float.MaxValue;
        bool found = false;

        foreach (MiniBallEntity player in team2.GetPlayers())
        {
            float d = Vector2.Distance(new Vector2(player.PosX, player.PosZ), ballPos);
            if (d < closestDist)
            {
                closestDist = d;
                bestPiece = player.Piece;
                found = true;
            }
        }

        if (!found) return null;

        Vector2 oppGoalPos = Flat(team1.goalZone.transform.position);
        Vector2 clearDir = (oppGoalPos - ballPos).normalized;
        return BuildMove(bestPiece, clearDir * MAX_FORCE);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Projects a 3-D world position onto the flat (X, Z) play plane.</summary>
    private static Vector2 Flat(Vector3 v) => new(v.x, v.z);

    private static MiniBallMove BuildMove(MiniBallPiece piece, Vector2 force)
        => new() { PieceId = piece, forceX = force.x, forceZ = force.y };

    /// <summary>Rotates a 2-D direction vector by <paramref name="degrees"/>.</summary>
    private static Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }
}
