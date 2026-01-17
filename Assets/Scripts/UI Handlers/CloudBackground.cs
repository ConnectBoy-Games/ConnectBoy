using UnityEngine;

public class CloudBackground : MonoBehaviour
{
    int direction = 1, speed = 10;
    [SerializeField] float limit;

    private RectTransform t;
    private Vector3 leftTarget = new(), rightTarget = new();
    private Vector3 startPoint;
    private float time = 0;

    void Start()
    {
        t = GetComponent<RectTransform>();
        leftTarget = new Vector3(t.position.x - limit, t.position.y, t.position.z);
        rightTarget = new Vector3(t.position.x + limit, t.position.y, t.position.z);
        speed = Random.Range(3, 10);
        startPoint = t.position;
        direction = (int)(Random.value * 100) % 2 == 0 ? -1 : 1;
    }

    void Update()
    {
        if (direction == 1)
        {
            t.position = Vector3.Lerp(startPoint, rightTarget, time * speed / 100f);
        }
        else if (direction == -1)
        {
            t.position = Vector3.Lerp(startPoint, leftTarget, time * speed / 100f);
        }
        time += Time.deltaTime;

        if (time >= 10)
        {
            direction *= -1;
            time = 0;
            speed = Random.Range(3, 10);
            startPoint = t.position;
        }
    }
}
