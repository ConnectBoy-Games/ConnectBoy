using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.UI;
using Wagr;

public class MenuPanel : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] TMP_Text displayName;
    [SerializeField] private Image displayImage;

    [Header("Notification")]
    [SerializeField] Button notificationButton;
    [SerializeField] private GameObject notificationDot;
    [SerializeField] private float pollInterval = 15f; // Check every 15 seconds

    private bool _isPolling = false;
    private HashSet<long> _processedInviteTimestamps = new HashSet<long>();
    public static event Action<MatchInvite> OnNewInviteReceived;

    void OnEnable()
    {
        StartPolling();

        if (GameManager.instance.accountManager.loginState == LoginState.loggedIn)
        {
            displayName.text = GameManager.instance.accountManager.playerProfile.Name;
            displayImage.sprite = GameManager.instance.faceManager.GetFace(GameManager.instance.accountManager.playerProfile.DpIndex); //Load the player's image
        }
        else
        {
            displayName.text = "Guest";
            notificationButton.interactable = false;
            displayImage.sprite = GameManager.instance.faceManager.GetFace(-1); //Load the default image
        }
    }

    void OnDisable()
    {
        StopPolling();
    }

    void Start()
    {
        OnNewInviteReceived += (invite) =>
        {
            notificationDot.SetActive(true);
            GameManager.instance.GetComponent<AudioManager>().PlayNotificationSound();
        };
    }

    public void StartPolling()
    {
        if (_isPolling || GameManager.instance.accountManager.loginState != LoginState.loggedIn) return;

        _isPolling = true;
        InvokeRepeating(nameof(PollForInvites), 1f, pollInterval);
    }

    public void StopPolling()
    {
        CancelInvoke(nameof(PollForInvites));
        _isPolling = false;
    }

    private async void PollForInvites()
    {
        try
        {
            // 1. Load the "invites" key from the player's public data
            var id = AuthenticationService.Instance.PlayerId;
            var args = new Dictionary<string, object>
            {
                { "playerId", id },
            };

            // Call the Cloud Code function
            var result = await CloudCodeService.Instance.CallEndpointAsync<CloudInvitesGetProxy>("GetInvites", args);

            if (result.success)
            {
                MatchInvite[] invites = JsonConvert.DeserializeObject<MatchInvite[]>(result.data.ToString());

                foreach (var invite in invites)
                {
                    // 2. Check if we've already handled this specific invite
                    if (!_processedInviteTimestamps.Contains(invite.timestamp))
                    {
                        _processedInviteTimestamps.Add(invite.timestamp);

                        // 3. Trigger the UI/Event
                        Debug.Log($"New Wager Invite from {invite.senderUsername}!");
                        NotificationDisplay.instance.DisplayMessage($"New Wager Invite from {invite.senderUsername}!", NotificationType.info, 2f);
                        OnNewInviteReceived?.Invoke(invite);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Polling failed: {e.Message}");
            NotificationDisplay.instance.DisplayMessage("Failed to poll for invites: " + e.Message, NotificationType.error, 2f);
        }
    }
}
