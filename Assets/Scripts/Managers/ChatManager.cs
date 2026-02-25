using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Transform chatHolder;
    [SerializeField] GameObject chatPrefab;

    [SerializeField] TMP_InputField chatField;

    private int lastChatId = -1;
    private float initialY;

    private void Update()
    {
        initialY = rectTransform.anchoredPosition.y; //Store the initial Y position of the panel
    }

    void FixedUpdate()
    {
        if (TouchScreenKeyboard.visible)
        {
            float keyboardHeight = TouchScreenKeyboard.area.height; // Get the keyboard height in screen space

            // Convert screen height to local UI space (approximate) // Adjust based on Canvas Scaler
            float shiftAmount = keyboardHeight / Screen.height * 1080; // Assuming 1080p ref

            rectTransform.anchoredPosition = new Vector2(0, initialY + shiftAmount);
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2(0, initialY);
        }
    }

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
