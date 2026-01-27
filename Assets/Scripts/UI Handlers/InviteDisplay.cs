using System;
using UnityEngine;

public class InviteDisplay : MonoBehaviour
{
    private static Guid gameSessionId;

    public void ShowInvite(Guid id)
    {
        gameSessionId = id;
        this.gameObject.SetActive(true);
    }

    public void AcceptGame()
    {
        //TODO: Proceed to the game scene
       
    }

    public void RejectGame()
    {
        gameObject.SetActive(false);
    }
}
