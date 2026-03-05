using UnityEngine;

/// <summary>
/// Archery Bot – calculates a world-space aim point to pass to Arrow.SetTarget.
///
/// Difficulty levels:
///   Low    – Aims roughly at the target centre with heavy random scatter.
///             The bot often misses the bullseye ring entirely.
///   Medium – Aims near the centre with moderate scatter. Consistently lands
///             on the board but rarely hits the bullseye.
///   High   – Uses the spawn-to-target geometry to calculate a precise aim
///             point, then adds only slight jitter so it nearly always hits
///             the inner rings.
/// </summary>
public class ArcheryBot
{
    private readonly BotDifficulty difficulty;

    public ArcheryBot() : this(BotDifficulty.low) { }

    public ArcheryBot(BotDifficulty difficulty)
    {
        this.difficulty = difficulty;
    }

    // ------------------------------------------------------------------ //
    //  Entry point – called by ArcheryManager.MakeAIMove()                //
    //                                                                      //
    //  Parameters that must be supplied by the caller:                     //
    //    targetCenter  – World-space centre of the archery target board.   //
    //                    Passed from ArcheryManager (e.g. a [SerializeField]
    //                    Transform on the target object).                   //
    //    targetRadius  – Outer radius of the scoring area on the board.    //
    //                    Used to bound random scatter (see Inspector field  //
    //                    on ArcheryManager for the target collider size).   //
    //    spawnPoint    – World-space position of the arrow spawn (leftSpawn //
    //                    or rightSpawn from ArcheryControl). Needed for the //
    //                    hard-difficulty trajectory calculation.            //
    // ------------------------------------------------------------------ //

    /// <summary>
    /// Returns the world-space aim point the bot should shoot at.
    /// Feed this directly into Arrow.SetTarget / ArcheryControl.ReleaseArrow.
    /// </summary>
    public Vector3 ThinkMove(
        Vector3 targetCenter,   // World-space centre of the archery board
        float   targetRadius,   // Outer radius of the scored ring area
        Vector3 spawnPoint)     // World-space arrow spawn position
    {
        return difficulty switch
        {
            BotDifficulty.low    => CalculateEasyShot   (targetCenter, targetRadius),
            BotDifficulty.medium => CalculateMediumShot (targetCenter, targetRadius),
            BotDifficulty.high   => CalculateHardShot   (targetCenter, targetRadius, spawnPoint),
            _                    => CalculateEasyShot   (targetCenter, targetRadius),
        };
    }

    // ================================================================== //
    //  Low difficulty                                                      //
    //  Aim vaguely in the direction of the board with large scatter.      //
    //  The bot misses the board frequently.                               //
    // ================================================================== //
    private Vector3 CalculateEasyShot(Vector3 targetCenter, float targetRadius)
    {
        // Scatter covers up to 2× the target radius – often off-board
        float scatterRadius = targetRadius * 2f;
        Vector2 randomOffset = Random.insideUnitCircle * scatterRadius;

        return new Vector3(
            targetCenter.x + randomOffset.x,
            targetCenter.y + randomOffset.y,
            targetCenter.z
        );
    }

    // ================================================================== //
    //  Medium difficulty                                                   //
    //  Aims at the board consistently but rarely the bullseye.            //
    //  Scatter is kept within the outer ring area.                        //
    // ================================================================== //
    private Vector3 CalculateMediumShot(Vector3 targetCenter, float targetRadius)
    {
        // Scatter within 60 % of the outer ring – always on the board
        float scatterRadius = targetRadius * 0.6f;
        Vector2 randomOffset = Random.insideUnitCircle * scatterRadius;

        return new Vector3(
            targetCenter.x + randomOffset.x,
            targetCenter.y + randomOffset.y,
            targetCenter.z
        );
    }

    // ================================================================== //
    //  High difficulty                                                     //
    //  Calculates a corrected aim point from the spawn position toward    //
    //  the board centre, then adds only a tiny jitter.                    //
    //  This simulates a skilled archer who reads wind and distance.       //
    // ================================================================== //
    private Vector3 CalculateHardShot(Vector3 targetCenter, float targetRadius, Vector3 spawnPoint)
    {
        // Direction from spawn to target centre (used for depth correction)
        Vector3 toTarget   = (targetCenter - spawnPoint).normalized;
        float   distance   = Vector3.Distance(spawnPoint, targetCenter);

        // Simulate a very slight arc drop (gravity component) along the vertical axis.
        // The further the target, the more the bot must aim slightly above centre.
        // Arrow.speed = 30 so travel time ≈ distance / 30 seconds.
        float travelTime   = distance / 30f;
        float gravityDrop  = 0.5f * Mathf.Abs(Physics.gravity.y) * travelTime * travelTime;

        // Build corrected aim point: centre + subtle upward compensation
        Vector3 correctedAim = targetCenter + Vector3.up * gravityDrop;

        // Add a very small precision jitter (≤ 8 % of radius) for realism
        float jitterRadius   = targetRadius * 0.08f;
        Vector2 jitter       = Random.insideUnitCircle * jitterRadius;

        return new Vector3(
            correctedAim.x + jitter.x,
            correctedAim.y + jitter.y,
            correctedAim.z
        );
    }
}
