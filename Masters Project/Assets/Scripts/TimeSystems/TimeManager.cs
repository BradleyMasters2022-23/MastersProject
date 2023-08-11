/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 17th, 2022
 * Last Edited - August 2nd, 2023 by Ben Schuster
 * Description - Manages the global time value with player controls
 * ================================================================================================
 */
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Events;
using System.Collections;
using Sirenix.OdinInspector;

/// <summary>
/// Manages player-controlled time abilities
/// </summary>
public class TimeManager : MonoBehaviour, IDifficultyObserver
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

    [SerializeField] private ChannelBool onActivateTimestopChannel;

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
    [SerializeField, ReadOnly] private AudioSource source;

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
    [SerializeField, Space(3)] private ResourceBarSO timeGaugeCoreData;

    /// <summary>
    /// Current amount of slow gauge
    /// </summary>
    private ResourceBar currSlowGauge;
    /// <summary>
    /// Current amount of slow gauge
    /// </summary>
    public float CurrSlowGauge
    {
        get { return currSlowGauge.CurrentValue(); }
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
    /// Whether or not the player is in a regen field
    /// </summary>
    public bool inRegenField;

    private List<TimeOrb> timeOrbBuffer = new List<TimeOrb>();

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

        currSlowGauge = new ResourceBar();
        currSlowGauge.Init(timeGaugeCoreData, FixedUpdateCalls);

        source = gameObject.GetComponent<AudioSource>();
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

        GlobalDifficultyManager.instance.Subscribe(this, difficultyTimeRecoveryKey);
    }

    private void OnDisable()
    {
        onStateChangeChannel.OnEventRaised -= ToggleInputs;

        if (instance == this)
        {
            slowInput.started -= ToggleSlow;
            slowInput.canceled -= ToggleSlow;
        }
        
        GlobalDifficultyManager.instance.Unsubscribe(this, difficultyTimeRecoveryKey);
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

                    if(!cheatMode)
                    {
                        DrainGauge();
                    }
                    break;
                }
            case TimeGaugeState.RECHARGING:
                {
                    TryResume();
                    break;
                }
            case TimeGaugeState.FROZEN:
                {
                    TryResume();

                    // If timer is up, begin recharging
                    //if (replenishDelayTimer.TimerDone())
                    //{
                    //    ChangeState(TimeGaugeState.RECHARGING);
                    //}

                    break;
                }
            case TimeGaugeState.EMPTIED:
                {
                    TryResume();

                    // If timer is up, begin recharging
                    //if (emptiedDelayTimer.TimerDone())
                    //{
                    //    ChangeState(TimeGaugeState.FROZEN);
                    //}

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
        // Debug.Log("[TimeManager] Switching from "  + currentState + " to " + _newState);

        switch (_newState)
        {
            case TimeGaugeState.IDLE:
                {
                    break;
                }
            case TimeGaugeState.SLOWING:
                {
                    // Debug.Log("Playing to slow time");

                    if(source != null)
                        source.Stop();
                    stopTime.PlayClip(source);

                    onActivateTimestopChannel.RaiseEvent(true);

                    break;
                }
            case TimeGaugeState.RECHARGING:
                {
                    break;
                }
            case TimeGaugeState.FROZEN:
                {
                    // If entering frozen state, reset timer
                    //replenishDelayTimer.ResetTimer(replenishDelay.Current);

                    if (source != null)
                        source.Stop();
                    startTime.PlayClip(source);
                    onActivateTimestopChannel.RaiseEvent(false);


                    break;
                }
            case TimeGaugeState.EMPTIED:
                {
                    // If entering emptied state, reset timer
                    //emptiedDelayTimer.ResetTimer();

                    //Debug.Log("Playing to resume time");

                    if (source != null)
                        source.Stop();
                    startTime.PlayClip(source);
                    onActivateTimestopChannel.RaiseEvent(false);


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

        // Reduce main gauge. If theres any remainder, than its empty
        float remainder = currSlowGauge.Decrease(DepleteAmount);
        if(remainder >0)
            ChangeState(TimeGaugeState.EMPTIED);

        // Drain the gauge, determine if state should change
        //if (currSlowGauge.CurrentValue() - DepleteAmount <= 0)
        //{
        //    currSlowGauge = 0;
        //    ChangeState(TimeGaugeState.EMPTIED);
        //}
        //else
        //{
        //    currSlowGauge -= DepleteAmount;
        //}
    }

    public void SetCheatMode(bool cheat)
    {
        cheatMode = cheat;
        if (cheatMode)
        {
            AddGauge(999);
        }
    }

    /// <summary>
    /// Add more energy to the time gauge
    /// </summary>
    /// <param name="amount">amount to add, in seconds</param>
    /// <returns>Whether anything was added</returns>
    public bool AddGauge(float amount)
    {
        // If Idle, or filled, then dont use
        if(currentState == TimeGaugeState.IDLE)
            return false;

        // adjust amount from readable value to system value
        amount *= FixedUpdateCalls * difficultyTimeRecoveryMod;
        float remainder = currSlowGauge.Increase(amount);

        // if any remainder, then its maxed and should change state
        if (remainder > 0 && currentState != TimeGaugeState.SLOWING)
        {
            ChangeState(TimeGaugeState.IDLE);
        }
        else if (remainder == 0 && currentState == TimeGaugeState.EMPTIED)
        {
            ChangeState(TimeGaugeState.FROZEN);
        }
        // Replenish the gauge, determine if state should change
        //if (currSlowGauge.CurrentValue() + amount >= currSlowGauge.MaxValue())
        //{
        //    currSlowGauge = maxGauge;
        //    ChangeState(TimeGaugeState.IDLE);
        //}
        //else
        //{
        //    currSlowGauge += amount;
        //}

        // Since some was used, return true
        return true;
    }

    public TimeGaugeState GetState()
    {
        return currentState;
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

    /// <summary>
    /// Drain the current player gauge by a set amount
    /// </summary>
    /// <param name="amountInTime">Amount to drain, in seconds</param>
    public void DrainGauge(float amountInTime)
    {
        float amt = amountInTime * FixedUpdateCalls;
        currSlowGauge.Decrease(amt);
        ChangeState(TimeGaugeState.FROZEN);
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

    #region Upgrade Functions

    public float MaxGauge()
    {
        return currSlowGauge.MaxValue();
    }
    public float GetBaseMax()
    {
        return timeGaugeCoreData._maxValue;
    }
    //public float UpgradeMaxGauge()
    //{
    //    return slowDuration.Current;
    //}
    public void UpgradeIncrementMaxTimeGauge(float increment)
    {
        //Debug.Log("New increment is " + increment);
        //slowDuration.ChangeVal(newMax);
        increment *= FixedUpdateCalls;

        if(increment >= 0)
            currSlowGauge.IncreaseMax(increment, true);
        else
            currSlowGauge.DecreaseMax(increment * -1);

        currentState = TimeGaugeState.RECHARGING;
    }

    public ResourceBar GetDataRef()
    {
        return currSlowGauge;
    }

    #endregion

    #region Time Buffer 

    public bool IsFull()
    {
        return BufferFull();
    }

    public void AddToBuffer(TimeOrb orb)
    {
        if(!timeOrbBuffer.Contains(orb))
            timeOrbBuffer.Add(orb);
    }

    public void RemoveFromBuffer(TimeOrb orb)
    {
        if (timeOrbBuffer.Contains(orb))
            timeOrbBuffer.Remove(orb);
    }

    private bool BufferFull()
    {
        float recoveryPot = 0;
        foreach (TimeOrb o in timeOrbBuffer.ToArray())
        {
            recoveryPot += o.GetAmt() * FixedUpdateCalls * difficultyTimeRecoveryMod;
        }

        return (currSlowGauge.CurrentValue() + recoveryPot) >= (currSlowGauge.MaxValue());
    }

    #endregion

    #region Difficulty - Time Orb Efficiency

    /// <summary>
    /// Difficulty modifier to apply to time recovery sources
    /// </summary>
    private float difficultyTimeRecoveryMod = 1;
    /// <summary>
    /// Key for looking up time recovery setting
    /// </summary>
    private const string difficultyTimeRecoveryKey = "TimeReplenishRate";

    public void UpdateDifficulty(float newModifier)
    {
        difficultyTimeRecoveryMod = newModifier;
    }

    #endregion
}
