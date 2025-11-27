using TMPro;
using UnityEngine;

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
}
