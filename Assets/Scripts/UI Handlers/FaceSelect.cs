using UnityEngine;
using UnityEngine.UI;

public class FaceSelect : MonoBehaviour
{
    [SerializeField] Image face;
    private FaceManager fm;

    void OnEnable()
    {
        fm = GameManager.instance.faceManager;
        LoginPanel.dpIndex = Random.Range(0, fm.Count());
        face.sprite = fm.GetFace(LoginPanel.dpIndex);
    }

    public void MoveLeft()
    {
        if(LoginPanel.dpIndex <= 0)
        {
            LoginPanel.dpIndex = 0;
        }
        else
        {
            LoginPanel.dpIndex--;
        }

        face.sprite = fm.GetFace(LoginPanel.dpIndex);
    }

    public void MoveRight()
    {
        if (LoginPanel.dpIndex >= fm.Count())
        {
            LoginPanel.dpIndex = fm.Count() - 1;
        }
        else
        {
            LoginPanel.dpIndex++;
        }

        face.sprite = fm.GetFace(LoginPanel.dpIndex);
    }
}
