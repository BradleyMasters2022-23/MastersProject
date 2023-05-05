/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 17th, 2022
 * Last Edited - November 4th, 2022 by Ben Schuster
 * Description - Manages the global time value with player controls
 * ================================================================================================
 */
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Windows;

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
    [Tooltip("Time it takes to switch between the two time states")]
    [SerializeField, Range(0f, 0.99f)] private float timeStoppedThreshold;
    [Tooltip("Sound if player stops time")]
    [SerializeField] private AudioClipSO stopTime;
    [Tooltip("Sound if player starts time")]
    [SerializeField] private AudioClipSO startTime;
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
    /// Threshold for timescale to be considered stopped time. Used for objects that otherwise struggle to adapt for other scales. 
    /// </summary>
    private static float stoppedThreshold = 0.1f;

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

    public static bool TimeStopped
    {
        get { return worldTimeScale <= stoppedThreshold;  }
    }

    public static float TimeStopThreshold
    {
        get { return stoppedThreshold; }
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

    /// <summary>
    /// Whether or not the player is in a regen field
    /// </summary>
    public bool inRegenField;

    #endregion

    #region Time System Observer

    private List<TimeObserver> timeChangeObservers;

    private bool stoppedLastFrame;

    #endregion

    public static TimeManager instance;

    private bool cheatMode;

    /// <summary>
    /// Initialize controls and starting values
    /// </summary>
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }


        // Initialize controls
        StartCoroutine(InitializeControls());

        timeChangeObservers = new List<TimeObserver>();
        stoppedLastFrame = false;

        // Initialize variables
        worldTimeScale = NormalTime;
        stoppedThreshold = timeStoppedThreshold;

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

    private IEnumerator InitializeControls()
    {
        while (GameManager.controls == null)
            yield return null;

        controller = GameManager.controls;

        slowInput = controller.PlayerGameplay.SlowTime;
        slowInput.started += ToggleSlow;
        slowInput.canceled += ToggleSlow;
        slowInput.Enable();

        yield return null;
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

        if(stoppedLastFrame && !TimeStopped)
        {
            OnTimeResume();
        }
        else if(!stoppedLastFrame && TimeStopped)
        {
            OnTimeStop();
        }

        stoppedLastFrame = TimeStopped;
    }

    #region State Management

    /// <summary>
    /// Change state based on player input
    /// </summary>
    /// <param name="c">callback context [ignorable]</param>
    private void ToggleSlow(InputAction.CallbackContext c)
    {
        // If started input, then can only slow
        if(c.started)
        {
            // Switch to appropriate state
            switch (currentState)
            {
                case TimeGaugeState.IDLE:
                    {
                        ChangeState(TimeGaugeState.SLOWING);
                        break;
                    }
                case TimeGaugeState.RECHARGING:
                    {
                        ChangeState(TimeGaugeState.SLOWING);
                        break;
                    }
                case TimeGaugeState.FROZEN:
                    {
                        ChangeState(TimeGaugeState.SLOWING);
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
                    source?.Stop();
                    stopTime.PlayClip(source);

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

                    source?.Stop();
                    startTime.PlayClip(source);

                    break;
                }
            case TimeGaugeState.EMPTIED:
                {
                    // If entering emptied state, reset timer
                    emptiedDelayTimer.ResetTimer();

                    source?.Stop();
                    startTime.PlayClip(source);

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
        // if cheat mode enabled, dont drain gauge
        if (cheatMode)
            return;

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


    #region Observer Stuff

    private void OnTimeStop()
    {
        TimeObserver[] observers = timeChangeObservers.ToArray();

        for (int i = 0; i < observers.Length; i++)
        {
            // Make sure observer still exists. If not, remove it
            if (observers[i] != null)
            {
                observers[i].OnStop();
            }
            else
            {
                timeChangeObservers.RemoveAt(i);
            }
        }
    }
    private void OnTimeResume()
    {
        TimeObserver[] observers = timeChangeObservers.ToArray();

        for (int i = 0; i < observers.Length; i++)
        {
            // Make sure observer still exists. If not, remove it
            if (observers[i] != null)
            {
                observers[i].OnResume();
            }
            else
            {
                timeChangeObservers.RemoveAt(i);
            }
        }
    }

    public void Subscribe(TimeObserver newSub)
    {
        if (!timeChangeObservers.Contains(newSub))
            timeChangeObservers.Add(newSub);
    }

    public void UnSubscribe(TimeObserver newSub)
    {
        if (timeChangeObservers.Contains(newSub))
            timeChangeObservers.Remove(newSub);
    }

    #endregion

    public void SetCheatMode(bool cheat)
    {
        cheatMode = cheat;
    }

    public void ChipRefillGauge()
    {
        RefillGauge();
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
            ChangeState(TimeGaugeState.IDLE);
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

    public float UpgradeMaxGauge()
    {
        return slowDuration.Current;
    }

    public void SetRegenTime(float regenMultiplier)
    {
        float temp = replenishTime.Current * regenMultiplier;
        replenishTime.ChangeVal(Mathf.Ceil(temp));
    }

    public void SetGaugeMax(float gaugeMultiplier)
    {
        float temp = slowDuration.Current * gaugeMultiplier;
        slowDuration.ChangeVal(Mathf.Ceil(temp));
        //currentState = TimeGaugeState.RECHARGING;
    }

    public void UpgradeSetGaugeMax(float newMax)
    {
        slowDuration.ChangeVal(Mathf.Ceil(newMax));
        currentState = TimeGaugeState.RECHARGING;
    }

    public void SetRegenDelay(float delayMultiplier)
    {
        float temp = replenishDelay.Current * delayMultiplier;
        replenishDelay.ChangeVal(Mathf.Ceil(temp));
        replenishDelayTimer.ResetTimer(replenishDelay.Current);
    }

    public bool IsFull()
    {
        return currSlowGauge >= (slowDuration.Current * FixedUpdateCalls);
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
