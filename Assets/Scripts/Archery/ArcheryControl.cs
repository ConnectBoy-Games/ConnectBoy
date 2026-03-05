using UnityEngine;
using UnityEngine.UI;

public class ArcheryControl : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;

    [Header("Crosshair Control")]
    [SerializeField] float releaseTimer = 6;
    [SerializeField] GameObject dummy;
    [SerializeField] GameObject crosshair;
    [SerializeField] GameObject shotTimer;
    [SerializeField] Image shotTimerColor;

    [Header("Crosshair Controls")]
    [Range(0.1f, 20f)]
    public float followSpeed = 10f;
    [SerializeField] private Transform leftSpawn;
    [SerializeField] private Transform rightSpawn;

    [Header("Wind Influence")]
    [Tooltip("How strongly the wind nudges the aim dummy per second (world units). " +
             "Higher values make aiming noticeably harder in strong wind.")]
    [SerializeField] private float windAimInfluence = 0.12f;

    private ArrowState arrowState = ArrowState.Idle;
    private float t = 0;

    private Vector2 delta = Vector2.zero;

    void Update()
    {
        ApplyWindToDummy();
        HandleTouchInput();
    }

    /// <summary>
    /// Drifts the aim dummy each frame by the current wind vector.
    /// This creates continuous, organic instability in the player's crosshair
    /// while they are holding their finger down – stronger gusts are harder to fight.
    /// </summary>
    private void ApplyWindToDummy()
    {
        if (dummy == null) return;
        if (WindSystem.Instance == null) return;

        Vector2 wind = WindSystem.Instance.CurrentWind;
        // Translate in XY (screen-space feel). Z is ignored – camera is looking down Z.
        dummy.transform.Translate(
            new Vector3(wind.x, wind.y, 0f) * windAimInfluence * Time.deltaTime,
            Space.World
        );
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            t += Time.deltaTime; //Start counting time finger is on screen
            delta = Input.GetTouch(0).deltaPosition; //Get the delta position of the finger
            Camera.main.fieldOfView = Mathf.Lerp(82, 95, t/0.5f); //Zoom in the camera

            if(Input.GetTouch(0).phase == TouchPhase.Ended || t > releaseTimer) //Finger has been lifted from the screen
            {
                ReleaseArrow();
            }

            if (t > releaseTimer / 2) //Update the shot timer UI
            {
                shotTimer.SetActive(true);
                shotTimerColor.fillAmount = (t - (releaseTimer / 2)) / (releaseTimer / 2);
            }

            dummy.transform.Translate(new Vector3(delta.x, delta.y, 0) * Time.deltaTime);
            FollowCrosshair(Vector3.zero); //Move the crosshair to follow the dummy object
        }
        else //Finger is off the screen
        {
            t = 0;
            shotTimer.SetActive(false);
            shotTimerColor.fillAmount = 0;

            if(arrowState == ArrowState.Idle)
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 82, 1); //Reset FOV
            }
        }
    }

    private void FollowCrosshair(Vector3 delta)
    {
        crosshair.transform.position = Vector3.MoveTowards(crosshair.transform.position, dummy.transform.position + delta, followSpeed * Time.deltaTime);
    }

    void ReleaseArrow()
    {
        arrowState = ArrowState.Released;
        var arrow = Instantiate(arrowPrefab, leftSpawn).GetComponent<Arrow>();
        arrow.SetTarget(crosshair.transform.position, ref arrowState);
        t = 0;
    }

    public enum ArrowState : byte
    {
        Idle,
        Released,
        Flying,
        Hit
    }
}
