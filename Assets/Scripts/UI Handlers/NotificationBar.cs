using TMPro;
using UnityEngine;

public class NotificationBar : MonoBehaviour
{
    /// <summary> A single instance of the Notification Bar </summary>
    public static NotificationBar instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Animator loadingAnimation;

    [SerializeField] private GameObject notificationBar;
    [SerializeField] private TMP_Text notificationText;


    public void DisplayMessage(string text, NotificationType type, float time = 5)
    {
        notificationText.text = text;

        switch (type)
        {
            case NotificationType.info:
                notificationText.color = Color.white;
                break;
            case NotificationType.warning:
                notificationText.color = Color.yellow;
                break;
            case NotificationType.error:
                notificationText.color = Color.magenta;
                break;
        }

        Invoke(nameof(CloseNotificationBar), time);
    }

    public void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
        loadingAnimation.Play(0);
    }

    public void CloseNotificationBar()
    {
        notificationBar.SetActive(false);
        notificationText.text = "";
        notificationText.color = Color.white;
    }

    public void CloseLoadingScreen()
    {
        loadingScreen.SetActive(false);
        loadingAnimation.StopPlayback();
    }
}

public enum NotificationType : byte
{
    info,
    warning,
    error,
}

