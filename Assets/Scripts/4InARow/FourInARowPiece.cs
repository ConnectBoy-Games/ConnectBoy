using UnityEngine;

public class FourInARowPiece : MonoBehaviour
{
    bool move = false;
    Vector3 targetPos, startPos;
    float t = 0, mag = 0;

    void Update()
    {
        if (move)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Slerp(startPos, targetPos, t / (0.015f * mag));
        }
    }

    public void PlacePiece(Vector3 pos)
    {
        startPos = transform.position;
        targetPos = pos;
        mag = (targetPos - startPos).magnitude;
        move = true;
        t = 0f;
    }
}
