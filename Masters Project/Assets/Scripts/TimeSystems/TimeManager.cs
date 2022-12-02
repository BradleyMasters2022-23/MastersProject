/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 17th, 2022
 * Last Edited - November 4th, 2022 by Ben Schuster
 * Description - Manages the global time value with player controls
 * ================================================================================================
 */
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.Rendering;

/// <summary>
/// Manages player-controlled time abilities
/// </summary>
public class TimeManager : MonoBehaviour
{
    public enum TimeGaugeState
    {
        IDLE,
        SLOWING,
        RECHARGING,
        FROZEN,
        EMPTIED
    }

    /// <summary>
    /// Current state of the Time Gauge
    /// </summary>
    [SerializeField] private TimeGaugeState currentState;

    public TimeGaugeState CurrState
    {
        get { return currentState; }
    }

    [Header("---Game Flow---")]
    [SerializeField] private ChannelGMStates onStateChangeChannel;

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
    [Tooltip("Sound if player stops time")]
    [SerializeField] private AudioClip stopTime;
    [Tooltip("Sound if player starts time")]
    [SerializeField] private AudioClip startTime;
    private AudioSource source;

    /// <summary>
    /// Normal time value
    /// </summary>
    private readonly float NormalTime = 1;

    /// <summary>
    /// Current time scale the player controls
    /// </summary>
    private static float worldTimeScale;

    /// <summary>
    /// Current time scale the player controls
    /// </summary>
    public static float WorldTimeScale
    {
        get { return worldTimeScale; }
    }

    /// <summary>
    /// The current deltaTime adjusted with player-controlled timescale
    /// </summary>
    public static float WorldDeltaTime
    {
        get { return worldTimeScale * Time.deltaTime; }
    }

    /// <summary>
    /// Time float to track transition lerp
    /// </summary>
    private float transitionLerp;

    #endregion

    #region Gauge Gameplay Variables

    [Header("=== Gauge Values ===")]

    [Tooltip("Duration the player can slow time for with full gauge. Input in seconds.")]
    [SerializeField, Space(3)] private UpgradableFloat slowDuration;

    [Tooltip("Time it takes for this gauge take to replenish.")]
    [SerializeField, Space(3)] private UpgradableFloat replenishTime;

    [Tooltip("How long does it take before time gauge begins refilling?")]
    [SerializeField, Space(3)] private UpgradableFloat replenishDelay;

    [Tooltip("When gauge is fully depleted, how long before player can use this gauge again?")]
    [SerializeField, Space(3)] private UpgradableFloat emptiedDelay;

    /// <summary>
    /// Current amount of slow gauge
    /// </summary>
    [SerializeField] private float currSlowGauge;
    /// <summary>
    /// Current amount of slow gauge
    /// </summary>
    public float CurrSlowGauge
    {
        get { return currSlowGauge; }
    }

    /// <summary>
    /// How many times is fixed update called per second
    /// </summary>
    public const float FixedUpdateCalls = 50;

    /// <summary>
    /// Rate at which the gauge depletes while slowing.
    /// </summary>
    private const float DepleteAmount = 1;

    /// <summary>
    /// Timer for tracking replenish delay [FROZEN state]
    /// </summary>
    private ScaledTimer replenishDelayTimer;

    /// <summary>
    /// Timer for tracking emptied delay [EMPTIED state]
    /// </summary>
    private ScaledTimer emptiedDelayTimer;

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
        worldTimeScale = NormalTime;

        slowDuration.Initialize();
        replenishTime.Initialize();
        replenishDelay.Initialize();
        emptiedDelay.Initialize();

        replenishDelayTimer = new ScaledTimer(replenishDelay.Current, false);
        emptiedDelayTimer = new ScaledTimer(emptiedDelay.Current, false);

        // Multiply slow duration seconds by update calls per second
        currSlowGauge = slowDuration.Current * FixedUpdateCalls;

        source = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        onStateChangeChannel.OnEventRaised += ToggleInputs;
    }

    private void OnDisable()
    {
        onStateChangeChannel.OnEventRaised -= ToggleInputs;

        if (slowInput.enabled)
            slowInput.Disable();
    }

    private void FixedUpdate()
    {
        StateFunctionCall();
    }

    #region State Management

    /// <summary>
    /// Change state based on player input
    /// </summary>
    /// <param name="c">callback context [ignorable]</param>
    private void ToggleSlow(InputAction.CallbackContext c)
    {
        Debug.Log(c.action);

        // If started input, then can only slow
        if(c.started)
        {
            // Switch to appropriate state
            switch (currentState)
            {
                case TimeGaugeState.IDLE:
                    {
                        ChangeState(TimeGaugeState.SLOWING);
                        //source.PlayOneShot(stopTime, 0.5f);
                        break;
                    }
                case TimeGaugeState.RECHARGING:
                    {
                        ChangeState(TimeGaugeState.SLOWING);
                        //source.PlayOneShot(stopTime, 0.5f);
                        break;
                    }
                case TimeGaugeState.FROZEN:
                    {
                        ChangeState(TimeGaugeState.SLOWING);
                        //source.PlayOneShot(stopTime, 0.5f);
                        break;
                    }
            }
        }
        // If canceled input, then can only resume
        else if(c.canceled)
        {
            switch (currentState)
            {
                case TimeGaugeState.SLOWING:
                    {
                        ChangeState(TimeGaugeState.FROZEN);
                        break;
                    }
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
            case TimeGaugeState.IDLE:
                {
                    TryResume();
                    break;
                }
            case TimeGaugeState.SLOWING:
                {
                    TrySlow();
                    DrainGauge();
                    break;
                }
            case TimeGaugeState.RECHARGING:
                {
                    TryResume();
                    RefillGauge();
                    break;
                }
            case TimeGaugeState.FROZEN:
                {
                    TryResume();

                    // If timer is up, begin recharging
                    if (replenishDelayTimer.TimerDone())
                    {
                        ChangeState(TimeGaugeState.RECHARGING);
                    }

                    break;
                }
            case TimeGaugeState.EMPTIED:
                {
                    TryResume();

                    // If timer is up, begin recharging
                    if (emptiedDelayTimer.TimerDone())
                    {
                        ChangeState(TimeGaugeState.RECHARGING);
                    }

                    break;
                }

        }
    }

    /// <summary>
    /// Change the state of the time gauge. Trigger any state-transition events.
    /// </summary>
    /// <param name="_newState">The new state to move to</param>
    private void ChangeState(TimeGaugeState _newState)
    {
        //Debug.Log("[TimeManager] Switching from "  + currentState + " to " + _newState);

        switch (_newState)
        {
            case TimeGaugeState.IDLE:
                {
                    break;
                }
            case TimeGaugeState.SLOWING:
                {
                    source.Stop();
                    source.PlayOneShot(stopTime, 0.5f);

                    break;
                }
            case TimeGaugeState.RECHARGING:
                {
                    break;
                }
            case TimeGaugeState.FROZEN:
                {
                    // If entering frozen state, reset timer
                    replenishDelayTimer.ResetTimer();

                    source.Stop();
                    source.PlayOneShot(startTime, 0.5f);

                    break;
                }
            case TimeGaugeState.EMPTIED:
                {
                    // If entering emptied state, reset timer
                    emptiedDelayTimer.ResetTimer();

                    source.Stop();
                    source.PlayOneShot(startTime, 0.5f);

                    break;
                }
        }

        currentState = _newState;
    }

    #endregion

    /// <summary>
    /// Try to slow down time to the slowest time value
    /// </summary>
    private void TrySlow()
    {
        // Increment lerp variable, clamp
        transitionLerp += Time.deltaTime;
        transitionLerp = Mathf.Clamp(transitionLerp, 0, slowTransitionTime);

        // If not yet at slowest time, continue lerping
        if (worldTimeScale != slowestTime)
        {
            worldTimeScale = Mathf.Lerp(NormalTime, slowestTime, transitionLerp / slowTransitionTime);
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
        if (worldTimeScale != NormalTime)
        {
            worldTimeScale = Mathf.Lerp(NormalTime, slowestTime, transitionLerp / slowTransitionTime);
        }
    }

    /// <summary>
    /// Drain the slow time gauge
    /// </summary>
    private void DrainGauge()
    {
        // Drain the gauge, determine if state should change
        if(currSlowGauge - DepleteAmount <= 0)
        {
            currSlowGauge = 0;
            ChangeState(TimeGaugeState.EMPTIED);
        }
        else
        {
            currSlowGauge -= DepleteAmount;
        }
    }

    /// <summary>
    /// Refill the slow time gauge
    /// </summary>
    private void RefillGauge()
    {
        // Calculate how much to replenish
        float maxGauge = slowDuration.Current * FixedUpdateCalls;
        float replenishAmount = (maxGauge / replenishTime.Current) / FixedUpdateCalls;

        // Replenish the gauge, determine if state should change
        if (currSlowGauge + replenishAmount >= maxGauge)
        {
            currSlowGauge = maxGauge;
            ChangeState(TimeGaugeState.IDLE);
        }
        else
        {
            currSlowGauge += replenishAmount;
        }
    }

    /// <summary>
    /// Add more energy to the time gauge
    /// </summary>
    /// <param name="amount">amount to add</param>
    /// <returns>Whether anything was added</returns>
    public bool AddGauge(float amount)
    {
        // If Idle, or filled, then dont use
        if(currentState == TimeGaugeState.IDLE)
            return false;

        float maxGauge = slowDuration.Current * FixedUpdateCalls;

        // Replenish the gauge, determine if state should change
        if (currSlowGauge + amount >= maxGauge)
        {
            currSlowGauge = maxGauge;
            //ChangeState(TimeGaugeState.IDLE);
        }
        else
        {
            currSlowGauge += amount;
        }

        // Since some was used, return true
        return true;
    }

    public TimeGaugeState GetState()
    {
        return currentState;
    }

    public float MaxGauge()
    {
        return slowDuration.Current * FixedUpdateCalls;
    }

    /// <summary>
    /// Toggle inputs if game pauses
    /// </summary>
    /// <param name="_newState">new state</param>
    private void ToggleInputs(GameManager.States _newState)
    {
        if (_newState == GameManager.States.GAMEPLAY
            || _newState == GameManager.States.HUB)
        {
            slowInput.Enable();
        }
        else
        {
            slowInput.Disable();
        }
    }
}
