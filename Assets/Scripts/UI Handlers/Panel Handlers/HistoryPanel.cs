using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryPanel : MonoBehaviour
{
    [SerializeField] private GameObject historyItemPrefab;
    [SerializeField] private Transform contentParent;

    void OnEnable()
    {
        LoadHistory();
    }

    void LoadHistory()
    {
        /*
        var historyList = GameManager.instance.accountManager.playerProfile.matchHistory;
        historyList.Reverse();

        foreach(var match in historyList)
        {
            var item = Instantiate(historyItemPrefab, contentParent);
            var itemScript = item.GetComponent<HistoryUnit>();
            itemScript.Setup(match);
        }
        */
    }

    void ClearHistory()
    {
        foreach(Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}
