using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives a simple on-screen wind indicator HUD element.
///
/// Expected hierarchy (assign in Inspector):
///   WindPanel (this component)
///     ├─ WindArrow   (Image — rotated to show wind direction)
///     ├─ WindBar     (Image, Image Type = Filled — fill shows strength)
///     └─ WindLabel   (TMP_Text  — e.g. "Wind: ↗ Strong")
///
/// Attach this component to the root panel containing those children.
/// </summary>
public class WindIndicator : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Image that rotates to show wind direction (point the arrow's UP = pointing right in the sprite).")]
    [SerializeField] private RectTransform windArrow;

    [Tooltip("Filled Image whose fillAmount represents wind strength (0 = calm, 1 = max).")]
    [SerializeField] private Image windStrengthBar;

    [Tooltip("Optional text label showing a wind descriptor.")]
    [SerializeField] private TMP_Text windLabel;

    [Header("Colours")]
    [SerializeField] private Color calmColour   = new Color(0.4f, 0.8f, 1.0f);
    [SerializeField] private Color mediumColour = new Color(1.0f, 0.8f, 0.2f);
    [SerializeField] private Color strongColour = new Color(1.0f, 0.3f, 0.2f);

    private void Update()
    {
        if (WindSystem.Instance == null) return;

        Vector2 wind          = WindSystem.Instance.CurrentWind;
        float   normStrength  = WindSystem.Instance.NormalisedStrength;

        // --- Arrow rotation ---
        // Rotate so the arrow graphic points in the wind direction (XY plane)
        if (windArrow != null && wind.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(wind.y, wind.x) * Mathf.Rad2Deg;
            windArrow.localRotation = Quaternion.Euler(0f, 0f, angle - 90f); // -90 because Unity UI "up" = 90°
        }

        // --- Strength bar ---
        if (windStrengthBar != null)
        {
            windStrengthBar.fillAmount = Mathf.Clamp01(normStrength);
            windStrengthBar.color = Color.Lerp(
                Color.Lerp(calmColour, mediumColour, normStrength * 2f),
                strongColour,
                Mathf.Max(0f, normStrength * 2f - 1f)
            );
        }

        // --- Text label ---
        if (windLabel != null)
        {
            string descriptor = normStrength < 0.25f ? "Calm"
                              : normStrength < 0.55f ? "Breeze"
                              : normStrength < 0.80f ? "Strong"
                              : "Gust!";
            windLabel.text = $"Wind: {descriptor}";
        }
    }
}
