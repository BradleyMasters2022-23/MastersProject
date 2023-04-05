using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

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

public class BossTarget : Target
{
    [SerializeField] private BossEvent[] phaseChangeEvents;
    private int phaseIndex = -1;

    [SerializeField] private BossEvent onDeathEvents;

    protected override void Awake()
    {
        base.Awake();
        StartBoss();
    }

    public void StartBoss()
    {
        Debug.Log("Starting boss encounter");
        _healthManager.onHealthbarLostEvents += CallTransition;
        phaseIndex = -1;
        CallTransition();
    }

    protected override void KillTarget()
    {
        _healthManager.onHealthbarLostEvents -= CallTransition;

        base.KillTarget();
    }

    protected override void DestroyObject()
    {
        // Instead of destroying object, do any events.
        // Actual death will happen inside of the events

        StartCoroutine("ExecuteEvents", onDeathEvents.OnTriggerEvent);
    }

    public void CallTransition()
    {
        StartCoroutine(Transition());
    }

    protected IEnumerator Transition()
    {
        // increment to next phase 
        phaseIndex++;

        // If there is a phase left, transition to it
        if(phaseIndex < phaseChangeEvents.Length)
        {
            Debug.Log($"[BOSS] Transitioning to phase {phaseChangeEvents[phaseIndex].stageName}");

            TimedEvent[] events = phaseChangeEvents[phaseIndex].OnTriggerEvent;
            yield return StartCoroutine("ExecuteEvents", events);
        }


        Debug.Log("[BOSS] Transition done");
        yield return null;
    }

    protected IEnumerator ExecuteEvents(TimedEvent[] events)
    {
        Debug.Log("Executing events");
        foreach (var e in events)
        {
            e.action.Invoke();
            yield return new WaitForSeconds(e.executeTime);
        }

        yield return null;
    }
}
