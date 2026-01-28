using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class NotificationPanel : MonoBehaviour
{
    public UnityAction backAction;

    [SerializeField] private Transform notificationHolder;
    [SerializeField] private GameObject notificationPrefab;

    void OnEnable()
    {
        LoadNotifications();
        InviteDisplay.instance.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) GoBack();
    }

    public void LoadNotifications()
    {
        ClearNotifications();
        _ = LoadInvites();
    }

    public async Task LoadInvites()
    {
        var data = await CloudSaveSystem.RetrieveSpecificData<List<Wagr.MatchInvite>>("invites");

        foreach (var inv in data) //Create Notification Box UI and set details here:
        {
            var notification = Instantiate(notificationPrefab, notificationHolder).GetComponent<NotificationBox>();
            notification.SetBoxDetails(inv);
        }
    }

    public void GoBack()
    {
        if (InviteDisplay.instance.gameObject.activeSelf)
        {
            InviteDisplay.instance.CloseInvite();
        }
        else
        {
            backAction.Invoke();
        }
    }

    public void ClearNotifications()
    {
        foreach (Transform child in notificationHolder)
        {
            Destroy(child.gameObject);
        }
    }
}
