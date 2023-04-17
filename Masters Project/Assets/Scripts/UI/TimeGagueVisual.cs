/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - October 26th, 2022 by Ben Schuster
 * Description - Observe player time and update timeBar
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeGagueVisual : MonoBehaviour
{
    /// <summary>
    /// Player to track time of
    /// </summary>
    private TimeManager time;
    /// <summary>
    /// Slider of timeBar
    /// </summary>
    private Slider timeBar;

    private void Awake()
    {
        time = FindObjectOfType<TimeManager>(true);
        timeBar = GetComponentInChildren<Slider>();
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
    }

    public void ResetMaxValue()
    {
        timeBar.maxValue = time.MaxGauge();
    }
}
