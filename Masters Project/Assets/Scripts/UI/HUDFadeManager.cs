using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDFadeManager : MonoBehaviour
{
    public static HUDFadeManager instance;
    private CanvasGroup fadeElement;

    [SerializeField] Image targetImg;
    [SerializeField] float fadeInTime = 0.3f;
    [SerializeField] float fadeOutTime = 1f;
    Color defaultColor;

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
        defaultColor = targetImg.color;
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

    public void SetImmediate(bool fade)
    {
        fadeElement.alpha = fade ? 1 : 0;
    }

    public void SetColor(Color c)
    {
        targetImg.color = c;
    }
    public void ResetColor()
    {
        targetImg.color = defaultColor;
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
