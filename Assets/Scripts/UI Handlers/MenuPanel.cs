using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.CloudSave;
using UnityEngine;
using Wagr;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] TMP_Text accountBalance;

    private void Start()
    {
        Invoke(nameof(SetBalance), 0.1f);
    }

    public void SetBalance()
    {
        accountBalance.text = GameManager.instance.accountManager.playerProfile.balance.ToString();
    }

    [SerializeField] private float pollInterval = 10f; // Check every 10 seconds
    private bool _isPolling = false;
    private HashSet<long> _processedInviteTimestamps = new HashSet<long>();

    // Event to notify your UI
    public static event Action<MatchInvite> OnNewInviteReceived;

    public void StartPolling()
    {
        if (_isPolling) return;
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
            // 1. Load the "invites" key from the player's own data
            var query = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "invites" });

            if (query.TryGetValue("invites", out var item))
            {
                List<MatchInvite> incomingInvites = item.Value.GetAs<List<MatchInvite>>();

                foreach (var invite in incomingInvites)
                {
                    // 2. Check if we've already handled this specific invite
                    if (!_processedInviteTimestamps.Contains(invite.timestamp))
                    {
                        _processedInviteTimestamps.Add(invite.timestamp);
                        
                        // 3. Trigger the UI/Event
                        Debug.Log($"New Wager Invite from {invite.senderUsername}!");
                        OnNewInviteReceived?.Invoke(invite);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Polling failed: {e.Message}");
        }
    }

}
