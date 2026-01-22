using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcheryControl : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;

    [Header("Crosshair Control")]
    [SerializeField] float releaseTimer = 6;
    [SerializeField] GameObject crosshair;
    [SerializeField] GameObject shotTimer;
    [SerializeField] Image shotTimerColor;

    private bool released = false;
    private float t = 0, m = 0;
    private Vector2 delta = Vector2.zero;
    private Vector3 targetPosition = Vector3.zero;

    void Start()
    {
        
    }

    void Update()
    {
        if (t > releaseTimer) ReleaseArrow();

        if(Input.touchCount > 0)
        {
            t += Time.deltaTime;
            m += Time.deltaTime;

            if(t > releaseTimer / 2)
            {
                shotTimer.SetActive(true);
                shotTimerColor.fillAmount = (t - (releaseTimer / 2) / (releaseTimer / 2));
            }

            delta = Input.GetTouch(0).deltaPosition;

            targetPosition = crosshair.transform.position + new Vector3(delta.x, delta.y) * Time.deltaTime;
            crosshair.transform.position = Vector3.Slerp(crosshair.transform.position, targetPosition, m/1);
        }
    }

    void ReleaseArrow()
    {
        released = true;
        t = 0;
    }
}
