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

    private ArrowState arrowState = ArrowState.Idle;
    private float t = 0;

    private Vector2 delta = Vector2.zero;

    void Update()
    {
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            t += Time.deltaTime; //Start counting time finger is on screen
            delta = Input.GetTouch(0).deltaPosition; //Get the delta position of the finger
            Camera.main.fieldOfView = Mathf.Lerp(60, 65, t/0.5f); //Zoom in the camera

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
                //Camera.main.fieldOfView = 60; //Reset FOV
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
