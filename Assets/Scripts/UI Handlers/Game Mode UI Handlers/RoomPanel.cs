using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour
{
    private int wagerRoom = -1;
    Tuple<int, int> room;

    [SerializeField] private List<Button> roomListing;
    [SerializeField] private Button playButton;

    void OnEnable()
    {
        wagerRoom = -1;
        playButton.interactable = false;
        foreach (var item in roomListing)
        {
            item.onClick.AddListener(() => 
            {
                item.GetComponent<Image>().CrossFadeColor(Color.cyan, 1f, true, true);
            });
        }
    }

    public void SetWager(int amount)
    {
        wagerRoom = amount;
        playButton.interactable = true;
    }
}
