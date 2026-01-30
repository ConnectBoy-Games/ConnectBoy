using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField] Transform chatHolder;
    [SerializeField] GameObject chatPrefab;

    [SerializeField] TMP_InputField chatField;

    private int lastChatId = -1;

    private void OnEnable()
    {
        InvokeRepeating(nameof(LoadChats), 0f, 5f); //Check for new chats every 5 seconds
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(LoadChats)); //Don't check for chats if the chat panel is not open
    }

    public async void SendChat()
    {
        string chat = chatField.text;
        if (chat == "") return;

        chatField.text = ""; //Clear the chat input field

        ChatMessage chatMessage = new ChatMessage
        {
            chatId = this.lastChatId,
            Message = chat,
            Username = GameManager.instance.accountManager.playerProfile.Name
        };

        var chats = await SessionHandler.SendSessionChat(GameManager.gameSession.sessionId.ToString(), chatMessage);
        GameManager.instance.GetComponent<AudioManager>().PlayChatSendSound();
        UpdateUI(chats);

    }

    public async void LoadChats()
    {
        var chats = await SessionHandler.GetSessionChat(GameManager.gameSession.sessionId.ToString(), lastChatId);
        UpdateUI(chats);
    }

    public void UpdateUI(List<ChatMessage> chats)
    {
        foreach (ChatMessage chatMessage in chats)
        {
            var chat = Instantiate(chatPrefab, chatHolder).GetComponent<ChatBox>(); //Add the chat textbox to the UI
            chat.SetUI(chatMessage.Username, chatMessage.Message); //Set the UI details of the chat message
            lastChatId = chatMessage.chatId; //Set the last chat id to the last loaded text
        }
    }
}
