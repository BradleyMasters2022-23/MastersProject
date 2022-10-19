/*
 * ================================================================================================
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
        Recharging,
        Frozen,
        Emptied
    }

    [ShowInInspector] private TimeGaugeState currentState;

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

    #region Time-related Variables

    [Header("=== Slowing ===")]

    [Tooltip("The slowest possible time slow value")]
    [SerializeField, Range(0, 1)] private float slowestTime;
    [Tooltip("Time it takes to switch between the two time states")]
    [SerializeField, Range(0.01f, 1)] private float slowTransitionTime;

    /// <summary>
    /// Normal time value
    /// </summary>
    private readonly float NormalTime = 1;

    /// <summary>
    /// Current time scale the player controls
    /// </summary>
    private static float worlTimeScale;

    /// <summary>
    /// Current time scale the player controls
    /// </summary>
    public static float WorlTimeScale
    {
        get { return worlTimeScale; }
    }

    /// <summary>
    /// The current deltaTime adjusted with player-controlled timescale
    /// </summary>
    public static float WorldDeltaTime
    {
        get { return worlTimeScale * Time.deltaTime; }
    }

    /// <summary>
    /// Time float to track transition lerp
    /// </summary>
    private float transitionLerp;

    #endregion

    #region Gauge-related Variables

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

    #endregion

    /// <summary>
    /// Initialize controls and starting values
    /// </summary>
    private void Awake()
    {
        // Initialize controls
        controller = new GameControls();
        slowInput = controller.PlayerGameplay.SlowTime;
        slowInput.started += ToggleSlow;
        slowInput.canceled += ToggleSlow;
        slowInput.Enable();

        // Initialize variables
        worlTimeScale = NormalTime;

        slowDuration.Initialize();
        depleteRate.Initialize();
        replenishRate.Initialize();
        replenishDelay.Initialize();
        emptiedDelay.Initialize();
    }

    private void OnDisable()
    {
        slowInput.Disable();
    }

    private void FixedUpdate()
    {
        StateFunctionCall();


    }

    /// <summary>
    /// Change state based on player input
    /// </summary>
    /// <param name="c">callback context [ignorable]</param>
    private void ToggleSlow(InputAction.CallbackContext c)
    {
        // Switch to appropriate state
        switch (currentState)
        {
            case TimeGaugeState.Idle:
                {
                    currentState = TimeGaugeState.Slowing;
                    break;
                }
            case TimeGaugeState.Slowing:
                {
                    currentState = TimeGaugeState.Idle;
                    break;
                }
        }
    }

    /// <summary>
    /// Call appropriate functionality based on current state.
    /// </summary>
    private void StateFunctionCall()
    {
        switch(currentState)
        {
            case TimeGaugeState.Slowing:
                {
                    TrySlow();
                    break;
                }
            case TimeGaugeState.Idle:
                {
                    TryResume();
                    break;
                }
        }
    }

    /// <summary>
    /// Try to slow down time to the slowest time value
    /// </summary>
    private void TrySlow()
    {
        // Increment lerp variable, clamp
        transitionLerp += Time.deltaTime;
        transitionLerp = Mathf.Clamp(transitionLerp, 0, slowTransitionTime);

        // If not yet at slowest time, continue lerping
        if (worlTimeScale != slowestTime)
        {
            worlTimeScale = Mathf.Lerp(NormalTime, slowestTime, transitionLerp / slowTransitionTime);
        }
    }

    /// <summary>
    /// Try to resume down time to the normal time value
    /// </summary>
    private void TryResume()
    {
        // Decrement lerp variable, clamp
        transitionLerp -= Time.deltaTime;
        transitionLerp = Mathf.Clamp(transitionLerp, 0, slowTransitionTime);

        // If not yet at normal time, continue lerping
        if (worlTimeScale != NormalTime)
        {
            worlTimeScale = Mathf.Lerp(NormalTime, slowestTime, transitionLerp / slowTransitionTime);
        }
    }
}
