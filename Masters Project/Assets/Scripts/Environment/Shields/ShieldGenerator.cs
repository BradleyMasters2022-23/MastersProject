/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 11, 2022
 * Last Edited - April 11, 2022 by Ben Schuster
 * Description - Implementation for a shield generator
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct ShieldFlickerData
{
    public bool flicker;

    [HideIf("@this.flicker == false")]
    public float[] downtimeIntervals;
    [HideIf("@this.flicker == false")]
    public float[] uptimeIntervals;
}

public class ShieldGenerator : MonoBehaviour
{
    [Tooltip("All buttons required to disable the shield")]
    [SerializeField] private TimedButton[] allButtons;
    [Tooltip("Reference to the shield gameobject")]
    [SerializeField] private GameObject shield;

    [Tooltip("Whether or not all buttons must stay active to keep the shield down")]
    [SerializeField] private bool maintainButtonActivation;

    [Tooltip("Indicators that play when the shield is disabled")]
    [SerializeField] private IIndicator[] onDisableIndicator;
    [Tooltip("Indicators that play when the shield is enabled")]
    [SerializeField] private IIndicator[] onEnableIndicator;

    [SerializeField] private ShieldFlickerData[] flickerPhaseData;

    private ShieldFlickerData currProfile;

    /// <summary>
    /// Whether the shield is currently active
    /// </summary>
    private bool active;

    /// <summary>
    /// Tracker for current transition coroutine
    /// </summary>
    private Coroutine currentRoutine;

    private Coroutine flickerRoutine;

    private void Awake()
    {
        active = true;
        currProfile = flickerPhaseData[0];
    }

    /// <summary>
    /// Check for changes in trigger state
    /// </summary>
    private void Update()
    {
        if(AllButtonsTriggered() && active)
        {
            if(currentRoutine!= null)
            {
                StopCoroutine(currentRoutine);
            }
            active = false;

            Debug.Log("Calling to disable shield");
            flickerRoutine = StartCoroutine(ShieldFlicker());
        }
        else if(!AllButtonsTriggered() && !active && maintainButtonActivation)
        {
            if (currentRoutine != null)
            {
                StopCoroutine(currentRoutine);
            }

            currentRoutine = StartCoroutine(EnableShield());
        }
    }

    /// <summary>
    /// Check if all buttons have been triggered
    /// </summary>
    /// <returns>Whether all buttons are currently triggered</returns>
    private bool AllButtonsTriggered()
    {
        foreach(var button in allButtons)
        {
            if (!button.Activated() && button.isActiveAndEnabled)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Enable the shield and perform any effects 
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnableShield()
    {
        Indicators.SetIndicators(onEnableIndicator, true);

        // more stuff can be used here
        
        shield.SetActive(true);
        //active = true;
        currentRoutine= null;
        yield return null;
    }

    /// <summary>
    /// Disable the shield and perform any effects
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableShield() 
    {
        Indicators.SetIndicators(onDisableIndicator, true);

        // more stuff can be used here

        shield.SetActive(false);
        //active = false;
        currentRoutine = null;
        yield return null;
    }

    private IEnumerator ShieldFlicker()
    {
        // If set to flicker, than flicker between intervals and loop
        if(currProfile.flicker)
        {
            // Get max index, which is the smallest size of the current data
            int maxIndex;
            if (currProfile.uptimeIntervals.Length <= currProfile.downtimeIntervals.Length)
                maxIndex = currProfile.uptimeIntervals.Length;
            else
                maxIndex = currProfile.downtimeIntervals.Length;

            // get starting index 
            int flickerIndex = 0;

            float downtime;
            float uptime;

            ScaledTimer timer = new ScaledTimer(0);

            while(true)
            {
                // Get current timing
                downtime = currProfile.downtimeIntervals[flickerIndex];
                uptime = currProfile.uptimeIntervals[flickerIndex];

                // start with downtime for responsiveness
                yield return StartCoroutine(DisableShield());
                timer.ResetTimer(downtime);
                yield return new WaitUntil(timer.TimerDone);

                yield return StartCoroutine(EnableShield());
                timer.ResetTimer(uptime);
                yield return new WaitUntil(timer.TimerDone);

                // Increment. If max is reached, then wrap around
                flickerIndex++;
                if (flickerIndex >= maxIndex)
                    flickerIndex = 0;

                yield return null;
            }
        }
        else // If no flicker, just turn it off
        {
            yield return StartCoroutine(DisableShield());
        }

        flickerRoutine = null;

        yield return null;
    }

    /// <summary>
    /// Reset the shield and all buttons
    /// </summary>
    public void ResetShield()
    {
        //if(currentRoutine!=null)
        //    StopCoroutine(currentRoutine);

        //if (flickerRoutine != null)
        //    StopCoroutine(flickerRoutine);

        StopAllCoroutines();
        active = true;
        currentRoutine = StartCoroutine(EnableShield());
        foreach(var button in allButtons)
            button.ResetButton();
    }

    public void SetPhaseData(int phase)
    {
        currProfile = flickerPhaseData[phase];
    }
}
