/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 25, 2022
 * Last Edited - April 25, 2022 by Ben Schuster
 * Description - A field that will regenerate the player's time gauge while in it
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TimeRegenField : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private float timePerSecond;
    private TimeManager timeGauge;

    [Header("Alert Text")]
    private WarningText warningText;
    [SerializeField, TextArea] string enterText;
    [SerializeField, TextArea] string exitText;

    private void Awake()
    {
        timeGauge = TimeManager.instance;
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if target is player and dont regen while slowing
        if(other.CompareTag("Player") && timeGauge.CurrState != TimeManager.TimeGaugeState.SLOWING)
        {
            // As this function is called every physics frame, no need for conversion!
            timeGauge.AddGauge(timePerSecond);
        }
    }

    private bool WarningTextInit()
    {
        if (warningText == null)
            warningText = WarningText.instance;

        return warningText != null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(WarningTextInit())
        {
            warningText.Play(enterText, false);
        }

        timeGauge.inRegenField = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (WarningTextInit())
        {
            warningText.Play(exitText, false);
        }

        timeGauge.inRegenField = false;
    }

    
}
