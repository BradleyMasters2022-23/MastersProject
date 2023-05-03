/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 28, 2022
 * Last Edited - April 28, 2022 by Ben Schuster
 * Description - Sequencer for timed events
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : TimeAffectedEntity
{
    [Tooltip("Sequence of events to execute when called")]
    [SerializeField] private TimedEvent[] events;

    /// <summary>
    /// Begin a sqeuence of events
    /// </summary>
    /// <param name="events"></param>
    /// <param name="timeAffected"></param>
    /// <returns></returns>
    public Coroutine ProcessSequence(TimedEvent[] events, bool timeAffected)
    {
        return StartCoroutine(ExecuteEvents(events, timeAffected));
    }

    /// <summary>
    /// Execute all events with time delays present
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    protected IEnumerator ExecuteEvents(TimedEvent[] events, bool timeAffected)
    {
        //Debug.Log("Executing events");
        ScaledTimer tracker = new ScaledTimer(0, false);
        foreach (var e in events)
        {
            if (timeAffected)
                tracker.SetModifier(Timescale);

            e.action.Invoke();
            tracker.ResetTimer(e.executeTime);
            yield return new WaitUntil(tracker.TimerDone);
        }

        yield return null;
    }

    /// <summary>
    /// Call this sequence to beign.
    /// </summary>
    public void BeginSequence()
    {
        ProcessSequence(events, Affected);
    }
}
