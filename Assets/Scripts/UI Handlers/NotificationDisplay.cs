using TMPro;
using UnityEngine;

public class NotificationDisplay : MonoBehaviour
{
    /// <summary> A single instance of the Notification Bar </summary>
    public static NotificationDisplay instance;

    [SerializeField] private TMP_Text notificationContent;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DisplayMessage(string text, NotificationType type = NotificationType.info, float time = -1)
    {
        gameObject.SetActive(true);
        LoadScreen.instance.HideScreen(); //Disable the load screen to show the notification
        notificationContent.text = text;

        switch (type)
        {
            case NotificationType.info:
                notificationContent.color = Color.green;
                GameManager.instance.GetComponent<AudioManager>().PlayNotificationSound();
                break;
            case NotificationType.warning:
                GameManager.instance.GetComponent<AudioManager>().PlayErrorSound();
                notificationContent.color = Color.yellow;
                break;
            case NotificationType.error:
                GameManager.instance.GetComponent<AudioManager>().PlayErrorSound();
                notificationContent.color = Color.red;
                break;
        }

        if (time > 0)
        {
            Invoke(nameof(CloseNotificationBar), time);
        }
    }

    public void CloseNotificationBar()
    {
        gameObject.SetActive(false);
        notificationContent.text = "";
        GameManager.instance.GetComponent<AudioManager>().PlayClickSound();
    }
}

public enum NotificationType : byte
{
    info,
    warning,
    error,
}


