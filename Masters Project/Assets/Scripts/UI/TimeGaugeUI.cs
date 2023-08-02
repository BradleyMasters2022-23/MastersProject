/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - August 2nd, 2023 by Ben Schuster
 * Description - Observe player time and update timeBar
 * ================================================================================================
 */
using UnityEngine;
using UnityEngine.UI;

public class TimeGaugeUI : ResourceBarUI
{
    /// <summary>
    /// Player to track time of
    /// </summary>
    private TimeManager time;

    [Header("Time Gauge")]

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

        _targetData = time.GetDataRef();

        flashTimer = new ScaledTimer(0.7f, false);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        // Determine conditions for flashing
        flashing = (_mainSlider.value == _mainSlider.maxValue) || (time.inRegenField && time.CurrState != TimeManager.TimeGaugeState.SLOWING);

        // If flashing, enable the image (if not already done) and update flash
        if (flashing)
        {
            if (!flashImage.enabled)
            {
                flashImage.enabled = true;
                flashTimer.ResetTimer();
            }

            FlashColor();
        }
        // Otherwise, make sure the flashing image is disabled
        else if (flashImage.enabled)
        {
            flashImage.enabled = false;
        }
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
