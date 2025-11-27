using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    [SerializeField] private Image image;

    public readonly UnityEvent playbackStarted = new();
    public readonly UnityEvent playbackEnded = new();

    private bool playing = false;

    private void Update()
    {
        if (playing && image.canvasRenderer.GetAlpha() == 0)
        {
            playing = false;
            playbackEnded?.Invoke();
        }
    }

    public void Play()
    {
        image.canvasRenderer.SetAlpha(1);
        playing = true;
        playbackStarted?.Invoke();
        image.CrossFadeAlpha(0, 1, false);
    }

    public void ClearCallbacks()
    {
        playbackEnded.RemoveAllListeners();
        playbackStarted.RemoveAllListeners();
    }
}
