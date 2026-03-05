using UnityEngine;

/// <summary>
/// Singleton that drives the wind simulation for the archery scene.
///
/// Wind is modelled as two independent Perlin-noise channels (X and Y) that
/// scroll through noise-space over time, producing smooth, organic gusts.
/// A "gust" multiplier ramps up and down periodically to simulate short bursts
/// of stronger wind.
///
/// Other scripts query the wind via WindSystem.Instance.CurrentWind.
/// </summary>
public class WindSystem : MonoBehaviour
{
    public static WindSystem Instance { get; private set; }

    [Header("Wind Strength")]
    [Tooltip("Baseline maximum wind displacement per second (world units).")]
    [SerializeField] private float maxWindStrength = 0.8f;

    [Tooltip("Minimum wind speed (fraction of max). Prevents total calm.")]
    [SerializeField] private float minWindFraction = 0.1f;

    [Header("Wind Variation")]
    [Tooltip("How fast the Perlin-noise position scrolls – controls how quickly wind direction changes.")]
    [SerializeField] private float noiseScrollSpeed = 0.3f;

    [Tooltip("How frequently gust bursts occur (seconds between gust peaks).")]
    [SerializeField] private float gustInterval = 4f;

    [Tooltip("How long a single gust lasts (seconds).")]
    [SerializeField] private float gustDuration = 1.2f;

    [Tooltip("How much a gust multiplies the base wind (e.g. 2 = double strength during a gust).")]
    [SerializeField] private float gustMultiplier = 2.0f;

    // --------------- Runtime state ---------------
    private float noiseOffsetX; // Unique noise offset per axis so X and Y differ
    private float noiseOffsetY;
    private float gustTimer;
    private float gustPhase; // 0..1, bell-curve through a gust lifetime

    /// <summary>World-space 2D wind vector (X = horizontal, Y = vertical drift) this frame.</summary>
    public Vector2 CurrentWind { get; private set; }

    /// <summary>Normalised strength 0-1 (useful for UI).</summary>
    public float NormalisedStrength => CurrentWind.magnitude / maxWindStrength;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Randomise noise offsets so each game session feels different
        noiseOffsetX = Random.Range(0f, 1000f);
        noiseOffsetY = Random.Range(0f, 1000f);

        gustTimer = Random.Range(0f, gustInterval); // Start at a random point in the gust cycle
    }

    private void Update()
    {
        float time = Time.time;

        // ---- Base wind from Perlin noise (remapped -1..1) ----
        float rawX = Mathf.PerlinNoise(noiseOffsetX + time * noiseScrollSpeed, 0f) * 2f - 1f;
        float rawY = Mathf.PerlinNoise(0f, noiseOffsetY + time * noiseScrollSpeed) * 2f - 1f;

        // Clamp low end so there is always at least minWindFraction of max
        float clampedX = Mathf.Sign(rawX) * Mathf.Max(Mathf.Abs(rawX), minWindFraction);
        float clampedY = Mathf.Sign(rawY) * Mathf.Max(Mathf.Abs(rawY), minWindFraction);

        Vector2 baseWind = new Vector2(clampedX, clampedY) * maxWindStrength;

        // ---- Gust burst ----
        gustTimer += Time.deltaTime;
        if (gustTimer >= gustInterval)
        {
            gustTimer = 0f;
        }
        // Map gust phase: peak at the centre of [0, gustDuration]
        gustPhase = 0f;
        if (gustTimer < gustDuration)
        {
            float t = gustTimer / gustDuration;                    // 0..1
            gustPhase = Mathf.Sin(t * Mathf.PI);                   // 0 → 1 → 0 bell curve
        }

        float gustBoost = 1f + gustPhase * (gustMultiplier - 1f);

        CurrentWind = baseWind * gustBoost;
    }
}
