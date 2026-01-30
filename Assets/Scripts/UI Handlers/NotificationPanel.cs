using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using UnityEngine;
using UnityEngine.Events;
using Wagr;

public class NotificationPanel : MonoBehaviour
{
    public UnityAction backAction;

    [SerializeField] private Transform notificationHolder;
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private InviteDisplay inviteDisplay;

    void OnEnable()
    {
        LoadNotifications();
        inviteDisplay.gameObject.SetActive(false);
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
        var id = AuthenticationService.Instance.PlayerId;
        try
        {
            var args = new Dictionary<string, object>
            {
                { "playerId", id },
            };

            // Call the Cloud Code function
            var result = await CloudCodeService.Instance.CallEndpointAsync<CloudInvitesGetProxy>("GetInvites", args);

            if (result.success)
            {
                MatchInvite[] invites = JsonConvert.DeserializeObject<MatchInvite[]>(result.data.ToString());

                foreach (var inv in invites) //Create Notification Box UI and set details here:
                {
                    var notification = Instantiate(notificationPrefab, notificationHolder).GetComponent<NotificationBox>();
                    notification.SetBoxDetails(inv);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get profile: {e.Message}");
            NotificationDisplay.instance.DisplayMessage($"Could not fetch public data: {e.Message}", NotificationType.error);
        }

        //var data = await CloudSaveSystem.RetrieveSpecificData<List<Wagr.MatchInvite>>("invites");
    }

    public void GoBack()
    {
        if (inviteDisplay.gameObject.activeSelf)
        {
            inviteDisplay.CloseInvite();
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
