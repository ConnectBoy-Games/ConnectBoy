using UnityEngine;
using UnityEngine.Events;

public class NotificationPanel : MonoBehaviour
{
    public UnityAction backAction;

    [SerializeField] private GameObject notificationHolder;

    void Start()
    {

    }

    public void LoadNotifications()
    {

    }

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

    public void ClearNotifications()
    {

    }

    public void ReloadNotifications()
    {

    }
}
