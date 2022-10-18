/* ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 17th, 2022
 * Last Edited - October 17th, 2022 by Ben Schuster
 * Description - Manages the global time value with player controls
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

/// <summary>
/// Manages player-controlled time abilities
/// </summary>
public class TimeManager : MonoBehaviour
{
    public enum TimeGaugeState
    {
        Idle,
        Slowing,
        Regenerating,
        Frozen,
        Emptied
    }

    private TimeGaugeState currentState;

    #region Inputs

    /// <summary>
    /// Reference to the game's control scheme
    /// </summary>
    private GameControls controller;
    /// <summary>
    /// Player input for slowing time
    /// </summary>
    private InputAction slowInput;

    #endregion

    /// <summary>
    /// Current timescale
    /// </summary>
    private static float worldTime;

    public static float WorldTime
    {
        get { return worldTime; }
    }

    [Header("=== Slowing ===")]

    [Tooltip("The slowest possible time slow value")]
    [SerializeField, Range(0, 1)] private float slowestTime;
    [Tooltip("Time it takes to switch between the two time states")]
    [SerializeField, Range(0, 2)] private float slowTransitionSpeed;

    /// <summary>
    /// Whether or not the time manager is slowing time
    /// </summary>
    private bool slowing;

    [Header("=== Gauge Values ===")]

    [Tooltip("Duration the player can slow time for with full gauge. Input in seconds.")]
    [SerializeField, Space(3)] private UpgradableFloat slowDuration;

    [Tooltip("How quickly does this value deplete.")]
    [SerializeField, Space(3)] private UpgradableFloat depleteRate;

    [Tooltip("How quickly does this value replenish.")]
    [SerializeField, Space(3)] private UpgradableFloat replenishRate;

    [Tooltip("How long does it take before time gauge begins refilling?")]
    [SerializeField, Space(3)] private UpgradableFloat replenishDelay;

    [Tooltip("When gauge is fully depleted, how long before player can use this gauge again?")]
    [SerializeField, Space(3)] private UpgradableFloat emptiedDelay;

    /// <summary>
    /// Whether or not the time gauge has been fully emptied
    /// </summary>
    private bool emptied;


    /// <summary>
    /// Initialize controls and starting values
    /// </summary>
    private void Awake()
    {
        controller = new GameControls();
        slowInput = controller.PlayerGameplay.SlowTime;

        slowing = false;
    }

    
    private void ToggleSlow()
    {
        // Don't toggle if gauge in empty state
        if(!emptied)
        {
            slowing = !slowing;
        }
    }

    private void FixedUpdate()
    {
        
    }
}
