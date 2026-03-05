using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public User teamName;
    [SerializeField] GoalZone goalZone;

    public List<List<Transform>> formations; //A list of formations(Formations are just a list of positions)
    public bool isActive = false; //Used to limit whose player pieces can be played

    public void SetFormation(int index)
    {

    }

    //Activates the indicator for every member of the team
    public void SetIndicator(bool value)
    {
        foreach (Transform piece in transform)
        {
            piece.GetComponent<PlayerPiece>().SetIndictator(value);
        }
    }

    public void SetPlayersStatus(bool value)
    {
        foreach (Transform piece in transform)
        {
            piece.GetComponent<PlayerPiece>().isPlayable = value;
        }
    }

    public List<Transform> GetFormation(int index)
    {
        return formations[index];
    }

    public void CheckGoal(Transform ball)
    {
        if(goalZone.Contains(ball.position))
        {
            //Player 2 Scores
        }
    }
}
