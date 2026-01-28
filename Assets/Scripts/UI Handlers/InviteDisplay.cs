using System;
using UnityEngine;
using UnityEngine.Events;

public class InviteDisplay : MonoBehaviour
{
    /// <summary> A single instance of the Invite Display Bar</summary>
    public static InviteDisplay instance;

    public UnityAction acceptInvite;
    public UnityAction rejectInvite;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowInvite()
    {
        gameObject.SetActive(true);
    }

    public void CloseInvite()
    {
        acceptInvite = CloseInvite;
        rejectInvite = CloseInvite;
        gameObject.SetActive(false);
    }

    public void AcceptGame()
    {
        acceptInvite?.Invoke();
        gameObject.SetActive(false);
    }

    public void RejectGame()
    {
        rejectInvite?.Invoke();
        gameObject.SetActive(false);
    }
}
