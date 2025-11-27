using UnityEngine;
using UnityEngine.Events;

public class ProfilePanel : MonoBehaviour
{
    public UnityAction backAction;

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            backAction.Invoke();
        }
    }

    public void GoBack()
    {
        backAction.Invoke();
    }
}
