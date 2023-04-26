/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - April 26th, 2022 by Ben Schuster
 * Description - Observe player time and update timeBar
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeGaugeUI : MonoBehaviour
{
    /// <summary>
    /// Player to track time of
    /// </summary>
    private TimeManager time;
    /// <summary>
    /// Slider of timeBar
    /// </summary>
    private Slider timeBar;

    [Tooltip("The color to flash the bar")]
    [SerializeField] private Color flashColor;
    [SerializeField] private Image flashImage;

    /// <summary>
    /// Timer to track the flashing intervals
    /// </summary>
    private ScaledTimer flashTimer;

    /// <summary>
    /// Whether or not the flash image is approaching the flash color
    /// </summary>
    private bool coloringFlash;

    /// <summary>
    /// Whether or not the bar is supposed to flash
    /// </summary>
    private bool flashing;

    private void Awake()
    {
        time = FindObjectOfType<TimeManager>(true);
        timeBar = GetComponentInChildren<Slider>();
        flashTimer = new ScaledTimer(0.7f, false);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // As time starts max
        timeBar.maxValue = time.MaxGauge();
        timeBar.value = time.CurrSlowGauge;
    }
    private void Update()
    {
        if (timeBar != null)
            timeBar.value = time.CurrSlowGauge;

        // Determine conditions for flashing
        flashing = (timeBar.value == timeBar.maxValue) || (time.inRegenField && time.CurrState != TimeManager.TimeGaugeState.SLOWING);

        // If flashing, enable the image (if not already done) and update flash
        if(flashing)
        {
            if (!flashImage.enabled)
            {
                flashImage.enabled = true;
                flashTimer.ResetTimer();
            }

            FlashColor();
        }
        // Otherwise, make sure the flashing image is disabled
        else if(flashImage.enabled)
        {
            flashImage.enabled = false;

        }
    }

    public void ResetMaxValue()
    {
        timeBar.maxValue = time.MaxGauge();
    }

    private void FlashColor()
    {
        // Determine if lerp is done and should reverse
        if(flashTimer.TimerDone())
        {
            coloringFlash = !coloringFlash;
            flashTimer.ResetTimer();
        }

        // Depending on direction, adjust alpha based on timer progress
        Color c = flashColor;
        if (coloringFlash)
        {
            c.a = flashTimer.TimerProgress();
        }
        else
        {
            c.a = 1 - flashTimer.TimerProgress();
        }

        // Apply new color
        flashImage.color = c;
    }
}
