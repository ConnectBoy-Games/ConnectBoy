using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MiniBall Bot – evaluates every bot player and picks the best kick.
///
/// Difficulty levels:
///   Low    – Each player tests a small set of random directions; picks the one that
///             gets the ball closest to the opponent goal.
///   Medium – Each player tests a structured angular sweep; also avoids own-goals.
///   High   – Full angular + force sweep using CustomSoccerPhysics simulation;
///             additionally tries to clear the ball away from the bot's own goal
///             when it is in a dangerous position.
/// </summary>
public class MiniBallBot
{
    private readonly BotDifficulty difficulty;

    // Bot controls Player 2 (pieces 6-10). Player 1 goal is on the negative-X side.
    // We assume Player2 shoots toward negative-X to score.
    // Adjust these values to match your actual scene layout.
    private static readonly Vector2 OpponentGoalPosition = new(-12f, 0f); // Player1's goal
    private static readonly Vector2 OwnGoalPosition      = new( 12f, 0f); // Player2's own goal

    // The pieces the bot controls
    private static readonly MiniBallPiece[] BotPieces =
    {
        MiniBallPiece.Player2_Piece1,
        MiniBallPiece.Player2_Piece2,
        MiniBallPiece.Player2_Piece3,
        MiniBallPiece.Player2_Piece4,
        MiniBallPiece.Player2_Piece5,
    };

    // Force constants – mirroring PlayerPiece which uses magnitude * 3 from drag distance.
    private const float MIN_FORCE    =  600f;
    private const float MAX_FORCE    = 1800f;
    private const float KICK_RADIUS  =   1.5f; // How close a player must be to the ball to kick it
    private const float GOAL_DANGER_RADIUS = 3.5f; // Distance from own goal that triggers defensive play (high only)

    public MiniBallBot() : this(BotDifficulty.low) { }

    public MiniBallBot(BotDifficulty difficulty)
    {
        this.difficulty = difficulty;
    }

    // ------------------------------------------------------------------ //
    //  Entry point – called by MiniBallManager.MakeAIMove()               //
    // ------------------------------------------------------------------ //

    /// <summary>
    /// Analyses the current game state and returns the best move for the bot.
    /// Returns null if no valid player can reach the ball.
    /// </summary>
    public MiniBallMove MakeMove(MiniBallState state)
    {
        if (state?.entities == null) return null;

        return difficulty switch
        {
            BotDifficulty.low    => ThinkEasy(state),
            BotDifficulty.medium => ThinkMedium(state),
            BotDifficulty.high   => ThinkHard(state),
            _                    => ThinkEasy(state),
        };
    }

    // ================================================================== //
    //  Difficulty: Low                                                     //
    //  Strategy  : Each reachable player tests a handful of random         //
    //              directions; picks whichever result gets the ball        //
    //              closest to the opponent's goal.                         //
    // ================================================================== //
    private MiniBallMove ThinkEasy(MiniBallState state)
    {
        MiniBallEntity ball = GetBall(state);
        if (ball == null) return null;

        Vector2 ballPos = new(ball.PosX, ball.PosY);

        MiniBallMove bestMove  = null;
        float        bestScore = float.MaxValue;

        foreach (MiniBallPiece piece in BotPieces)
        {
            MiniBallEntity player = GetEntity(state, piece);
            if (player == null) continue;

            Vector2 playerPos = new(player.PosX, player.PosY);
            if (Vector2.Distance(playerPos, ballPos) > KICK_RADIUS) continue;

            // Test 8 evenly-spaced directions + noise
            for (int i = 0; i < 8; i++)
            {
                float angle     = i * 45f + Random.Range(-20f, 20f);
                float force     = Random.Range(MIN_FORCE * 0.5f, MAX_FORCE * 0.6f);
                Vector2 kickDir = AngleToVector(angle);
                Vector2 kickVec = kickDir * force;

                // Lightweight estimate: where does the ball end up after this kick?
                Vector2 estimatedBallEnd = EstimateBallEndPos(ballPos, kickVec);
                float   distToGoal       = Vector2.Distance(estimatedBallEnd, OpponentGoalPosition);

                if (distToGoal < bestScore)
                {
                    bestScore = distToGoal;
                    bestMove  = BuildMove(piece, kickVec);
                }
            }
        }

        return bestMove;
    }

    // ================================================================== //
    //  Difficulty: Medium                                                  //
    //  Strategy  : Structured 36-direction sweep per player.              //
    //              Penalises directions that would send the ball near      //
    //              the bot's own goal (own-goal avoidance).               //
    // ================================================================== //
    private MiniBallMove ThinkMedium(MiniBallState state)
    {
        MiniBallEntity ball = GetBall(state);
        if (ball == null) return null;

        Vector2 ballPos = new(ball.PosX, ball.PosY);

        MiniBallMove bestMove  = null;
        float        bestScore = float.MaxValue;

        foreach (MiniBallPiece piece in BotPieces)
        {
            MiniBallEntity player = GetEntity(state, piece);
            if (player == null) continue;

            Vector2 playerPos = new(player.PosX, player.PosY);
            if (Vector2.Distance(playerPos, ballPos) > KICK_RADIUS) continue;

            // 36 directions (every 10°), 2 force levels
            float[] forces = { MIN_FORCE * 0.7f, MAX_FORCE * 0.75f };
            for (int i = 0; i < 36; i++)
            {
                float angle     = i * 10f;
                Vector2 kickDir = AngleToVector(angle);

                foreach (float force in forces)
                {
                    Vector2 kickVec           = kickDir * force;
                    Vector2 estimatedBallEnd  = EstimateBallEndPos(ballPos, kickVec);

                    float distToGoal  = Vector2.Distance(estimatedBallEnd, OpponentGoalPosition);
                    float distOwnGoal = Vector2.Distance(estimatedBallEnd, OwnGoalPosition);

                    // Penalise own-goal risk
                    float score = distToGoal + Mathf.Max(0f, 6f - distOwnGoal) * 5f;

                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestMove  = BuildMove(piece, kickVec);
                    }
                }
            }
        }

        return bestMove;
    }

    // ================================================================== //
    //  Difficulty: High                                                    //
    //  Strategy  : Full physics simulation (CustomSoccerPhysics) for each //
    //              player × direction × force combination.                 //
    //              Adds DEFENSIVE mode: if the ball is dangerously close   //
    //              to the bot's own goal, the nearest player clears it.   //
    // ================================================================== //
    private MiniBallMove ThinkHard(MiniBallState state)
    {
        MiniBallEntity ball = GetBall(state);
        if (ball == null) return null;

        Vector2 ballPos = new(ball.PosX, ball.PosY);

        // ----- DEFENSIVE: clear the ball if it threatens own goal -----
        if (Vector2.Distance(ballPos, OwnGoalPosition) < GOAL_DANGER_RADIUS)
        {
            MiniBallMove clearMove = TryClearBall(state, ball, ballPos);
            if (clearMove != null) return clearMove;
        }

        // ----- OFFENSIVE: full simulation sweep -----
        MiniBallMove bestMove  = null;
        float        bestScore = float.MaxValue;

        // Build a shared entities list for simulation (deep-copied each run)
        List<MiniBallEntity> baseEntities = BuildEntityList(state);
        Collider[] walls = FindFieldWalls();

        foreach (MiniBallPiece piece in BotPieces)
        {
            MiniBallEntity player = GetEntity(state, piece);
            if (player == null) continue;

            Vector2 playerPos = new(player.PosX, player.PosY);
            if (Vector2.Distance(playerPos, ballPos) > KICK_RADIUS) continue;

            // 36 directions × 3 force levels
            float[] forces = { MIN_FORCE * 0.6f, MIN_FORCE, MAX_FORCE };

            for (int i = 0; i < 36; i++)
            {
                float angle     = i * 10f;
                Vector2 kickDir = AngleToVector(angle);

                foreach (float force in forces)
                {
                    Vector2 kickVec = kickDir * force;

                    // Deep-copy entities for this simulation run
                    List<MiniBallEntity> simEntities = DeepCopy(baseEntities);

                    var frames = CustomSoccerPhysics.SimulateSoccerTurn(
                        simEntities,
                        piece,
                        kickVec,
                        walls
                    );

                    // Score: distance of ball from opponent goal in the final frame
                    Vector2 finalBallPos = GetFinalBallPos(frames, ball.Piece);
                    float distToGoal  = Vector2.Distance(finalBallPos, OpponentGoalPosition);
                    float distOwnGoal = Vector2.Distance(finalBallPos, OwnGoalPosition);

                    // Penalise own-goal risk more aggressively at hard level
                    float score = distToGoal + Mathf.Max(0f, 5f - distOwnGoal) * 10f;

                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestMove  = BuildMove(piece, kickVec);
                    }
                }
            }
        }

        return bestMove;
    }

    // ================================================================== //
    //  Defensive Clear (High difficulty only)                             //
    //  Finds the closest bot player to the ball and kicks it away from   //
    //  the own goal toward the centre / opponent side.                    //
    // ================================================================== //
    private MiniBallMove TryClearBall(MiniBallState state, MiniBallEntity ball, Vector2 ballPos)
    {
        MiniBallPiece bestPiece    = MiniBallPiece.Player2_Piece1;
        float         closestDist  = float.MaxValue;
        bool          foundPlayer  = false;

        foreach (MiniBallPiece piece in BotPieces)
        {
            MiniBallEntity player = GetEntity(state, piece);
            if (player == null) continue;

            float d = Vector2.Distance(new Vector2(player.PosX, player.PosY), ballPos);
            if (d < closestDist && d <= KICK_RADIUS)
            {
                closestDist = d;
                bestPiece   = piece;
                foundPlayer = true;
            }
        }

        if (!foundPlayer) return null;

        // Kick direction: away from own goal (toward opponent goal), with slight upward variance
        Vector2 clearDirection = (OpponentGoalPosition - ballPos).normalized;
        float   clearForce     = MAX_FORCE;

        return BuildMove(bestPiece, clearDirection * clearForce);
    }

    // ================================================================== //
    //  Helpers                                                             //
    // ================================================================== //

    private static MiniBallEntity GetBall(MiniBallState state)
    {
        foreach (var e in state.entities)
            if (e.Piece == MiniBallPiece.Ball) return e;
        return null;
    }

    private static MiniBallEntity GetEntity(MiniBallState state, MiniBallPiece piece)
    {
        foreach (var e in state.entities)
            if (e.Piece == piece) return e;
        return null;
    }

    private static MiniBallMove BuildMove(MiniBallPiece piece, Vector2 force) =>
        new MiniBallMove { PieceId = piece, forceX = force.x, forceY = force.y };

    private static Vector2 AngleToVector(float angleDeg) =>
        new(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));

    /// <summary>
    /// Fast linear estimate of where the ball will come to rest after a kick,
    /// using exponential drag (no collision simulation).
    /// </summary>
    private static Vector2 EstimateBallEndPos(Vector2 startPos, Vector2 kickVec)
    {
        const float friction   = 0.97f;
        const float minSpeed   = 0.01f;
        Vector2     pos        = startPos;
        Vector2     vel        = kickVec;
        int         maxSteps   = 500;

        for (int s = 0; s < maxSteps && vel.magnitude > minSpeed; s++)
        {
            pos += vel * Time.fixedDeltaTime;
            vel *= friction;
        }
        return pos;
    }

    private static List<MiniBallEntity> BuildEntityList(MiniBallState state)
    {
        var list = new List<MiniBallEntity>(state.entities.Length);
        foreach (var e in state.entities) list.Add(e);
        return list;
    }

    private static List<MiniBallEntity> DeepCopy(List<MiniBallEntity> src)
    {
        var copy = new List<MiniBallEntity>(src.Count);
        foreach (var e in src)
        {
            copy.Add(new MiniBallEntity
            {
                Piece = e.Piece,
                PosX  = e.PosX,
                PosY  = e.PosY,
                velX  = e.velX,
                velY  = e.velY,
            });
        }
        return copy;
    }

    /// <summary>
    /// Reads the final position of the ball from the last simulation frame.
    /// </summary>
    private static Vector2 GetFinalBallPos(
        List<Dictionary<string, Vector2>> frames,
        MiniBallPiece ballPiece)
    {
        if (frames == null || frames.Count == 0) return Vector2.zero;

        string key = ballPiece.ToString();
        var    last = frames[^1];
        return last.TryGetValue(key, out Vector2 pos) ? pos : Vector2.zero;
    }

    /// <summary>
    /// Attempts to find wall colliders in the scene. Returns an empty array if scene
    /// objects are not accessible (e.g., during unit tests).
    /// </summary>
    private static Collider[] FindFieldWalls()
    {
        var wallObjects = GameObject.FindGameObjectsWithTag("Wall");
        if (wallObjects == null || wallObjects.Length == 0)
            return System.Array.Empty<Collider>();

        var colliders = new List<Collider>(wallObjects.Length);
        foreach (var go in wallObjects)
        {
            if (go.TryGetComponent(out Collider col))
                colliders.Add(col);
        }
        return colliders.ToArray();
    }
}
