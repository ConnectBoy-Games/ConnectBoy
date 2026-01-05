using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
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
    
    public async Task PollForInvites()
    {
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "invites" });

        if (data.TryGetValue("invites", out var item))
        {
            List<Wagr.MatchInvite> invites = item.Value.GetAs<List<Wagr.MatchInvite>>();
            foreach (var inv in invites)
            {
                Debug.Log($"{inv.senderUsername} challenged you to a Type {inv.matchType} match!");
                Debug.Log($"Wager: {inv.wagerAmount} coins.");
                
                // Trigger your UI Popup here:
                // uiPopup.Show(inv.senderUsername, inv.wagerAmount, inv.matchId);
            }
        }
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
