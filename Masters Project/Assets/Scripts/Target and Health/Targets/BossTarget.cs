/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 5, 2022
 * Last Edited - April 5, 2022 by Ben Schuster
 * Description - Boss target that manages phase transitions
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

#region Special Structures

[System.Serializable]
public struct TimedEvent
{
    public UnityEvent action;
    public float executeTime;
}

[System.Serializable]
public struct BossEvent
{
    [Space(5)]
    public string stageName;
    [TableList(CellPadding = 3)]
    [PropertySpace(SpaceAfter = 10, SpaceBefore = 5)]
    public TimedEvent[] OnTriggerEvent;
    
}
#endregion


public class BossTarget : Target
{
    [Header("Boss Stuff")]

    [Tooltip("Reference to the invulnerable shield")]
    [SerializeField] private ShieldGenerator shieldManager;

    /// <summary>
    /// List of events that occur when this target takes damage
    /// </summary>
    private UnityEvent onDamagedEvents;

    /// <summary>
    /// When damaged, how long until its shield reactivates
    /// </summary>
    [SerializeField] private float damagedReactivateShieldDelay;

    /// <summary>
    /// Tracker for current delay
    /// </summary>
    private Coroutine delayedRoutine;

    protected override void Awake()
    {
        base.Awake();
        // this is for testing, remove later
        StartBoss();
    }

    protected override void KillTarget()
    {
        if (delayedRoutine != null)
            StopCoroutine(delayedRoutine);

        _healthManager.onHealthbarLostEvents -= CallTransition;
        onDamagedEvents?.RemoveAllListeners();
        base.KillTarget();
    }

    public override void RegisterEffect(float dmg)
    {
        onDamagedEvents?.Invoke();

        base.RegisterEffect(dmg);
    }

    protected override void DestroyObject()
    {
        // Instead of destroying object, do any events.
        // Actual death will happen inside of the events
        StartCoroutine("ExecuteEvents", onDeathEvents.OnTriggerEvent);
    }

    #region Phase management

    [SerializeField] private BossEvent[] phaseChangeEvents;
    private int phaseIndex = -1;

    [SerializeField] private BossEvent onDeathEvents;

    /// <summary>
    /// Start the boss encounter, transition to first phase
    /// </summary>
    public void StartBoss()
    {
        //Debug.Log("Starting boss encounter");
        onDamagedEvents = new UnityEvent();

        _healthManager.onHealthbarLostEvents += CallTransition;
        phaseIndex = -1;
        CallTransition();
    }
    
    /// <summary>
    /// Public call for transition phase change
    /// </summary>
    public void CallTransition()
    {
        StartCoroutine(Transition());
    }

    /// <summary>
    /// Attempt to transition to the next stage, if possible
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Transition()
    {
        // increment to next phase 
        phaseIndex++;

        // If there is a phase left, transition to it
        if(phaseIndex < phaseChangeEvents.Length)
        {
            TimedEvent[] events = phaseChangeEvents[phaseIndex].OnTriggerEvent;
            yield return StartCoroutine("ExecuteEvents", events);
        }

        yield return null;
    }

    /// <summary>
    /// Execute all events with time delays present
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    protected IEnumerator ExecuteEvents(TimedEvent[] events)
    {
        //Debug.Log("Executing events");
        ScaledTimer tracker = new ScaledTimer(0);
        foreach (var e in events)
        {
            e.action.Invoke();
            tracker.ResetTimer(e.executeTime);
            yield return new WaitUntil(tracker.TimerDone);
        }

        yield return null;
    }

    #endregion


    public void SetReactivateShieldOnDamage()
    {
        onDamagedEvents.AddListener(DamagedShieldReactivation);
    }

    public void DamagedShieldReactivation()
    {
        delayedRoutine = StartCoroutine(DelayedActivation(damagedReactivateShieldDelay));
    }

    private IEnumerator DelayedActivation(float delay)
    {
        ScaledTimer t = new ScaledTimer(delay);
        yield return new WaitUntil(t.TimerDone);

        shieldManager.ResetShield();
        delayedRoutine = null;
    }

    public void SetNewRechargeDelay(float delay)
    {
        damagedReactivateShieldDelay= delay;
    }
}
