using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    [SerializeField] private GameObject indicator;

    private bool isDragging = false;
    private Vector3 startPosition;
    private Vector3 currentPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDragging)
        {
            currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseDown()
    {
        isDragging = true;
        startPosition = transform.position;
    }

    private void OnMouseUp()
    {
        var rb = GetComponent<Rigidbody>();
        var magnitude = -2 * (currentPosition - startPosition).magnitude;
        var direction = (currentPosition - startPosition).normalized;
        rb.AddForce(magnitude * direction, ForceMode.VelocityChange);
    }

    public void SetIndictator(bool state)
    {
        indicator.SetActive(state);
    }
}
