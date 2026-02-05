using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBallManager : MonoBehaviour
{
    [SerializeField] private Transform clientPlayerHolder; //The parent object that holds the client player's pieces
    [SerializeField] private Transform otherPlayerHolder; //Can hold either bot's or opponent pieces

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Board Placement
    private void SwitchTurns()
    {
        foreach (Transform piece in otherPlayerHolder)
        {
            piece.GetComponent<PlayerPiece>().SetIndictator(true);
        }
    }

    /// <summary>Places the player's pieces according to a predefined formation</summary>
    private void PlacePieces(int form)
    {

    }
    #endregion
}
