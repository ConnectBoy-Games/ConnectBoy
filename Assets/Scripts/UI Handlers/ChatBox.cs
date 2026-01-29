using TMPro;
using UnityEngine;

public class ChatBox : MonoBehaviour
{
    [SerializeField] TMP_Text username;
    [SerializeField] TMP_Text message;

    public void SetUI(string name, string chat)
    {
        username.text = name;
        message.text = chat;
    }
}
