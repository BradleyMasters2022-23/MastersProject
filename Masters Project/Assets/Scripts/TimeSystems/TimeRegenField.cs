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
    [Tooltip("How much time given to the player every second they're in the field")]
    [SerializeField] private float timePerSecond;
    /// <summary>
    /// Reference to the time gauge
    /// </summary>
    private TimeManager timeGauge;

    [Header("Communication")]
    [Tooltip("Warning text displayed upon entering the field")]
    [SerializeField, TextArea] string enterText;
    [Tooltip("Warning text displayed upon exiting the field")]
    [SerializeField, TextArea] string exitText;
    /// <summary>
    /// Reference to the warning text controller
    /// </summary>
    private WarningText warningText;

    
    private void Awake()
    {
        timeGauge = TimeManager.instance;
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if target is player and dont regen while slowing
        if(other.CompareTag("Player") && timeGauge.CurrState != TimeManager.TimeGaugeState.SLOWING)
        {
            timeGauge.AddGauge(timePerSecond / 50);
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
        // Dont do anything if not the player
        if (!other.CompareTag("Player")) return;

        if(WarningTextInit() && (MapLoader.instance == null || MapLoader.instance.LoadState != LoadState.Loading))
        {
            warningText.Play(enterText, false);
        }

        timeGauge.inRegenField = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // Dont do anything if not the player
        if (!other.CompareTag("Player")) return;

        if (WarningTextInit())
        {
            warningText.Play(exitText, false);
        }

        timeGauge.inRegenField = false;
    }
}
