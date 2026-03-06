using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public User teamName;
    public MiniBallManager manager;
    public GoalZone goalZone;
    public List<Formation> formations;

    public bool isActive = false;
    public int scoreCount = 0;
    public bool reportedMove = false; //Used to prevent multiple move reports in a single turn

    void Start()
    {
        goalZone.teamManager = this;

        foreach (Transform player in transform)
        {
            player.GetComponent<PlayerPiece>().teamManager = this;
        }
    }

    public void SetFormation(int index)
    {
        scoreCount = 0;

        //Index selects the formation to be used
        index = index % formations.Count;

        for (int i = 0; i < 5; i++)
        {
            transform.GetChild(i).position = formations[index].positions[i].position;
        }
    }

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

    public List<MiniBallEntity> GetPlayers()
    {
        List<MiniBallEntity> temp = new();

        foreach (Transform player in transform)
        {
            MiniBallEntity entity = new MiniBallEntity
            {
                Piece = player.GetComponent<PlayerPiece>().piece,
                PosX = player.transform.position.x,
                PosZ = player.transform.position.z,
            };
            temp.Add(entity);
        }

        return temp;
    }

    public List<Transform> GetFormation(int index)
    {
        return formations[index].positions;
    }

    public void MakeMove()
    {
        if (reportedMove == false)
        {
            reportedMove = true;
            manager.MadeMove(); //Call the MadeMove() function in the manager to record the move and switch turns
        }
    }

    public bool CheckGoal(Transform ball)
    {
        CheckPlayers();

        if (goalZone.Contains(ball.position) || scoreCount > 0)
        {
            ball.GetComponent<Ball>().PlayEffect(); //Play the star effect to indicate a goal
            return true;
        }
        return false;
    }

    //Check to remove any of the players stuck in the post
    public void CheckPlayers()
    {
        foreach (Transform player in transform)
        {
            if (goalZone.Contains(player.position))
            {
                player.position = GetFormation(Random.Range(0, formations.Count))[Random.Range(0, 5)].position;
            }
        }
    }

    /// <summary>Formations are just a list of positions</summary>
    [System.Serializable]
    public class Formation
    {
        public List<Transform> positions;
    }
}
