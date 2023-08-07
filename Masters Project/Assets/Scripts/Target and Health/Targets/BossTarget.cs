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
    public AudioClipSO eventAudio;

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

    [SerializeField] private ChannelVoid playerKilledChannel;

    [SerializeField] private AudioClipSO bossStartNormalLine;
    [SerializeField] private AudioClipSO bossStartKilledPlayerLine;
    [SerializeField] private AudioClipSO playerKilledLine;

    /// <summary>
    /// List of events that occur when this target takes damage
    /// </summary>
    private UnityEvent onDamagedEvents;

    /// <summary>
    /// When damaged, how long until its shield reactivates
    /// </summary>

    [PropertySpace(SpaceAfter = 15)]
    [SerializeField] private float damagedReactivateShieldDelay;

    /// <summary>
    /// Tracker for current delay
    /// </summary>
    private Coroutine delayedRoutine;

    protected override void KillTarget()
    {
        if (delayedRoutine != null)
            StopCoroutine(delayedRoutine);

        GlobalStatsManager.data.killedByBossLastRun = -1;
        playerKilledChannel.OnEventRaised -= OnPlayerKilled;
        _healthManager.onHealthbarLostEvents -= CallTransition;
        onDamagedEvents?.RemoveAllListeners();
        base.KillTarget();
    }

    public override void RegisterEffect(float dmg, Vector3 origin)
    {
        onDamagedEvents?.Invoke();

        base.RegisterEffect(dmg, origin);
    }

    protected override void DestroyObject()
    {
        // Instead of destroying object, do any events.
        // Actual death will happen inside of the events
        StartCoroutine("ExecuteEvents", onDeathEvents.OnTriggerEvent);
    }

    #region Phase management

    [TabGroup("Standardized Events")]
    [TableList(CellPadding = 3)]
    [SerializeField] private BossEvent standardPhaseChangeEvents;

    [TabGroup("Phase Change Events")]
    [SerializeField] private BossEvent[] phaseChangeEvents;
    private int phaseIndex = -1;

    [TabGroup("Death Events")]
    [TableList(CellPadding = 3)]
    [SerializeField] private BossEvent onDeathEvents;

    /// <summary>
    /// Start the boss encounter, transition to first phase
    /// </summary>
    public void StartBoss()
    {
        //Debug.Log("Starting boss encounter");
        onDamagedEvents = new UnityEvent();

        // if player was killed last run, play unique hook
        // this is the first boss, so check boss killed index 0
        if (GlobalStatsManager.data.killedByBossLastRun == 0)
        {
            bossStartKilledPlayerLine.PlayClip(audioSource);
        }
        // otherwise, play normal line
        else
        {
            bossStartNormalLine.PlayClip(audioSource);
        }

        playerKilledChannel.OnEventRaised += OnPlayerKilled;
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
    /// when player dies, play special line
    /// </summary>
    private void OnPlayerKilled()
    {
        playerKilledChannel.OnEventRaised -= OnPlayerKilled;
        GlobalStatsManager.data.killedByBossLastRun = 0;
        playerKilledLine.PlayClip(audioSource);
    }

    /// <summary>
    /// Attempt to transition to the next stage, if possible
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Transition()
    {
        _healthManager.ToggleGodmode(true);

        // increment to next phase 
        phaseIndex++;

        // If there is a phase left, transition to it
        if(phaseIndex < phaseChangeEvents.Length)
        {
            // perform standard events
            standardPhaseChangeEvents.eventAudio.PlayClip(audioSource, true);
            TimedEvent[] events = standardPhaseChangeEvents.OnTriggerEvent;
            yield return StartCoroutine(ExecuteEvents(events));

            // perform special changes
            phaseChangeEvents[phaseIndex].eventAudio.PlayClip(audioSource);
            events = phaseChangeEvents[phaseIndex].OnTriggerEvent;
            yield return StartCoroutine("ExecuteEvents", events);
        }

        _healthManager.ToggleGodmode(false);

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

    public void DeathExplosionKnockback()
    {
        Target t = PlayerTarget.p;

        if(t != null)
        {
            t.Knockback(300, 65, _center.position);
        }
    }
}
