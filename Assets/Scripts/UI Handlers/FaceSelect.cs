using UnityEngine;
using UnityEngine.UI;

public class FaceSelect : MonoBehaviour
{
    public static int dpIndex;

    [SerializeField] Image face;
    private FaceManager fm;

    void Start()
    {
        fm = GameManager.instance.faceManager;
        dpIndex = Random.Range(0, fm.Count());
        face.sprite = fm.GetFace(dpIndex);
    }

    public void UpdateUI() => face.sprite = fm.GetFace(dpIndex);

    public void MoveLeft()
    {
        if(dpIndex <= 0)
        {
            dpIndex = 0;
        }
        else
        {
            dpIndex--;
        }

        face.sprite = fm.GetFace(dpIndex);
    }

    public void MoveRight()
    {
        if (dpIndex >= fm.Count() - 1)
        {
            dpIndex = fm.Count() - 1;
        }
        else
        {
            dpIndex++;
        }

        face.sprite = fm.GetFace(dpIndex);
    }
}
