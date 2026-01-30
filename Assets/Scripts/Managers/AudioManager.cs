using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource m_AudioSource;

    public bool sfx { get; private set; }
    public bool vibrate { get; private set; }
    public float volume { get; private set; }

    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip invalidSound;
    [SerializeField] private AudioClip acceptedSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip wobbleSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip drawSound;
    [SerializeField] private AudioClip defeatSound;
    [SerializeField] private AudioClip placeSound;
    [SerializeField] private AudioClip notificationSound;
    [SerializeField] private AudioClip chatSendSound;

    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();

        //Get default audio values
        sfx = PlayerPrefs.GetInt("sfx", 1) == 1;
        vibrate = PlayerPrefs.GetInt("vibrate", 1) == 1;
        volume = PlayerPrefs.GetFloat("volume", 1);
    }

    public void SetSfx(bool value)
    {
        PlayerPrefs.SetInt("sfx", value ? 1 : 0);
        sfx = value;
    }

    public void SetVibrate(bool value)
    {
        PlayerPrefs.SetInt("vibrate", value ? 1 : 0);
        vibrate = value;
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("volume", Mathf.Clamp(value, 0, 1));
        volume = value;
    }

    public void PlayClickSound()
    {
        if (sfx) m_AudioSource.PlayOneShot(clickSound);
    }

    public void PlayErrorSound()
    {
        if (sfx) m_AudioSource.PlayOneShot(errorSound);
    }

    public void PlayInvalidSound()
    {
        if (sfx) m_AudioSource.PlayOneShot(invalidSound);
    }
    
    public void PlayAcceptSound()
    {
        if (sfx) m_AudioSource.PlayOneShot(acceptedSound);
    }

    public void PlayWobbleSound()
    {
        if (sfx) m_AudioSource.PlayOneShot(wobbleSound);
    }

    public void PlayDrawSound()
    {
        if (sfx) m_AudioSource.PlayOneShot(drawSound);
    }

    public void PlayNotificationSound()
    {
        if (sfx) m_AudioSource.PlayOneShot(notificationSound);
    }

    public void PlayDefeatSound()
    {
        if (sfx) m_AudioSource.PlayOneShot(defeatSound);
    }

    public void PlayVictorySound()
    {
        if (sfx) m_AudioSource.PlayOneShot(victorySound);
    }

    public void PlayChatSendSound()
    {
        if (sfx) m_AudioSource.PlayOneShot(chatSendSound);
    }

    public void PlayPlaceSound()
    {
        if (sfx) m_AudioSource.PlayOneShot(placeSound);
    }
}
