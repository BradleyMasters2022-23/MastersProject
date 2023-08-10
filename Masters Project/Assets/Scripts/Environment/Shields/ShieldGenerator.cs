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
    [SerializeField] private ShieldTarget shield;

    [Tooltip("Whether or not all buttons must stay active to keep the shield down")]
    [SerializeField] private bool maintainButtonActivation;

    [SerializeField] private AudioClipSO shieldRebootSFX;

    [Tooltip("Indicators that play when the shield is disabled")]
    [SerializeField] private IIndicator[] powerOffIndicators;
    [Tooltip("Indicators that play when the shield is enabled")]
    [SerializeField] private IIndicator[] powerOnIndicators;
    [SerializeField] private Animator[] shieldAnimator;

    private AudioSource source;

    /// <summary>
    /// Whether the shield is currently active
    /// </summary>
    private bool active;

    private void Awake()
    {
        source= GetComponent<AudioSource>();
        active = true;
    }

    /// <summary>
    /// Check for changes in trigger state
    /// </summary>
    private void Update()
    {
        if(AllButtonsTriggered() && active)
        {
            active = false;
            DisableShield();
            Debug.Log("shield gen set to power down");
            foreach(Animator a in shieldAnimator)
                a.SetTrigger("PowerDown");
        }
        else if(!AllButtonsTriggered() && !active && maintainButtonActivation)
        {
            active = true;
            EnableShield();
            Debug.Log("shield gen set to power up");

            foreach (Animator a in shieldAnimator)
                a.SetTrigger("PowerUp");
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
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Enable the shield and perform any effects 
    /// </summary>
    /// <returns></returns>
    private void EnableShield()
    {
        Indicators.SetIndicators(powerOnIndicators, true);

        // more stuff can be used here
        shield.enabled = true;
    }

    /// <summary>
    /// Disable the shield and perform any effects
    /// </summary>
    /// <returns></returns>
    private void DisableShield() 
    {
        Indicators.SetIndicators(powerOffIndicators, true);

        // more stuff can be used here
        shield.enabled = false;
    }


    /// <summary>
    /// Reset the shield and all buttons
    /// </summary>
    public void ResetShield()
    {
        StopAllCoroutines();
        active = true;
        
        shieldRebootSFX.PlayClip(source);

        EnableShield();
        foreach (var button in allButtons)
            button.ResetButton();
    }

    public void LockGenerators()
    {
        //Debug.Log("Locking all generators");
        foreach (var button in allButtons)
            button.SetLock(true);
    }

    public void UnlockGenerators()
    {
        //Debug.Log("Unlocking all generators");
        foreach (var button in allButtons)
            button.SetLock(false);
    }

    public void SetButtonsDead()
    {
        foreach (var button in allButtons)
            button.Die();
    }
}
