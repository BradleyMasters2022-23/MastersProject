using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDFadeManager : MonoBehaviour
{
    public static HUDFadeManager instance;
    private CanvasGroup fadeElement;

    [SerializeField] float fadeInTime = 0.3f;
    [SerializeField] float fadeOutTime = 1f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            return;
        }

        fadeElement = GetComponent<CanvasGroup>();
    }
    /// <summary>
    /// Fade the HUD in to block the camera
    /// </summary>
    public IEnumerator FadeIn()
    {
        return FadeRoutine(0, 1, fadeInTime);
    }
    /// <summary>
    /// Fade the HUD out to clear the camera
    /// </summary>
    public IEnumerator FadeOut()
    {
        return FadeRoutine(1, 0, fadeInTime);
    }

    private IEnumerator FadeRoutine(float start, float end, float dur)
    {
        ScaledTimer t = new ScaledTimer(dur, false);

        while(!t.TimerDone())
        {
            fadeElement.alpha = Mathf.Lerp(start, end, t.TimerProgress());
            yield return null;
        }

        fadeElement.alpha = end;
    }
}
