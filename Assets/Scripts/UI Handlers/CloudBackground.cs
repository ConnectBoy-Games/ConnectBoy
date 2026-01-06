using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CloudBackground : MonoBehaviour
{
    int direction = 1, speed = 10;
    [SerializeField] float limit;
    private RectTransform t;

    void Start()
    {
        t = GetComponent<RectTransform>();
        direction = ((int)(Random.value * 100) % 2 == 0) ? -1 : 1;  
        speed = Random.Range(7, 18);
    }

    void Update()
    {
        t.position = new Vector3(t.position.x + (direction * speed * Time.deltaTime), t.position.y, t.position.z);    
    
        if(t.position.x > limit || t.position.x < -limit)
        {
            direction *= -1;
            speed = Random.Range(8, 12);
        }
    }
}
