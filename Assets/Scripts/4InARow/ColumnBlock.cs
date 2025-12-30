using UnityEngine;
using UnityEngine.Events;

public class ColumnBlock : MonoBehaviour
{
    [SerializeField] private Transform bottomPosition;
    [SerializeField] private GameObject redPiece, bluePiece;

    public UnityEvent onClick;

    private void OnMouseUp()
    {
        onClick.Invoke();
    }

    public void PlacePiece(int type)
    {
        switch (type)
        {
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
    }
}
