using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] private List<Button> roomListing;
    [SerializeField] private Button playButton;

    private int wagerRoom = -1;

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

    public void EnterRoom()
    {
        //Load the actual game level and set the scene accordingly
        GameManager.instance.GoToSelectedGame();
    }
}
