using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    public MiniBallPiece piece;
    [SerializeField] private GameObject indicator;

    private bool isDragging = false;
    private Vector3 startPosition;
    private Vector3 currentPosition;

    void Update()
    {
        if (isDragging)
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
