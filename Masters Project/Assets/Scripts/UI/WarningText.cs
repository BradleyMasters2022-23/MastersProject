using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Masters.UI;
using Sirenix.OdinInspector;

public class WarningText : MonoBehaviour
{
    public static WarningText instance;

    [Tooltip("Where to display the target to")]
    [SerializeField] private TextMeshProUGUI targetDisplay;
    [Tooltip("The text that displays for the player")]
    [SerializeField, TextArea] private string warningText;
    [Tooltip("The delay between printing each character")]
    [SerializeField] private float delayTime;
    [Tooltip("The total time to display the text for")]
    [SerializeField] private float displayTime;


    [SerializeField] private AudioClipSO alertAudio;

    [SerializeField] private Color alertColor1;
    [SerializeField] private Color alertColor2;
    [SerializeField] private float alertColorInterval;
    private Color currColor;
    private Color tarColor;
    private ScaledTimer colorTimer;

    private bool active;
    private Coroutine displayRoutine;
    private ScaledTimer activeTimer;
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        active = false;
        activeTimer = new ScaledTimer(displayTime + (delayTime * warningText.Length), false);
        colorTimer = new ScaledTimer(alertColorInterval, false);

        targetDisplay.text = "";
    }

    /// <summary>
    /// Play the warning text with default text
    /// </summary>
    public void Play()
    {
        Play(warningText);
    }

    /// <summary>
    /// Play the warning text with new text
    /// </summary>
    /// <param name="newText">New text to be played</param>
    public void Play(string newText)
    {
        active = true;
        activeTimer.ResetTimer();
        alertAudio.PlayClip();

        currColor = alertColor1;
        tarColor = alertColor2;
        colorTimer.ResetTimer();

        displayRoutine = StartCoroutine(targetDisplay.SlowTextLoad(newText, delayTime));
    }

    private void Update()
    {
        if(active && activeTimer.TimerDone())
        {
            active = false;
            displayRoutine = null;

            // unload the text, x3 faster than the load speed
            StartCoroutine(targetDisplay.SlowTextUnload(delayTime/2f));
        }
        else if(active)
        {
            FlipColors();
        }
    }

    private void OnDisable()
    {
        if(displayRoutine != null)
        {
            StopCoroutine(displayRoutine);
        }
    }

    private void FlipColors()
    {
        if(colorTimer.TimerDone())
        {
            colorTimer.ResetTimer();

            Color b = currColor;
            currColor = tarColor;
            tarColor = b;
        }

        targetDisplay.color = Color.Lerp(currColor, tarColor, colorTimer.TimerProgress());
    }
}
