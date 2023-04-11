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

    /// <summary>
    /// Whether the shield is currently active
    /// </summary>
    private bool active;

    /// <summary>
    /// Tracker for current transition coroutine
    /// </summary>
    private Coroutine currentRoutine;

    private void Awake()
    {
        active = true;
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

            Debug.Log("Calling to disable shield");
            currentRoutine = StartCoroutine(DisableShield());
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
        active = true;
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
        active = false;
        currentRoutine = null;
        yield return null;
    }

    /// <summary>
    /// Reset the shield and all buttons
    /// </summary>
    public void ResetShield()
    {
        if(currentRoutine!=null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(EnableShield());
        foreach(var button in allButtons)
            button.ResetButton();
    }
}
