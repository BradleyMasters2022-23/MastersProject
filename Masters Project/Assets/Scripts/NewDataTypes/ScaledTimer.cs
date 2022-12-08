/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 19th, 2022
 * Last Edited - October 19th, 2022 by Ben Schuster
 * Description - An independent timer class. Tracks its own time automatically.
 * ================================================================================================
 */
using UnityEngine;
using Masters.TimerSystem;

public class ScaledTimer 
{
    /// <summary>
    /// The target duration for this timer, in seconds.
    /// </summary>
    private float targetTime;
    /// <summary>
    /// The cached start time of this timer
    /// </summary>
    private float startTime;

    /// <summary>
    /// whether or not this timer is actually scaled
    /// </summary>
    private bool scaled;

    /// <summary>
    /// Create a new timer with 
    /// </summary>
    /// <param name="_targetTime">Target time for this value</param>
    /// <param name="_scaled">Whether this timer is scaled or not. Default true.</param>
    public ScaledTimer(float _targetTime, bool _scaled = true)
    {
        scaled = _scaled;
        if (scaled)
        {
            startTime = CoreTimeline.instance.ScaledTimeline;
        }
        else
        {
            // TODO - make using non real time value and delta time instead
            startTime = Time.realtimeSinceStartup;
        }

        targetTime = _targetTime;
    }

    /// <summary>
    /// Reset the active timer.
    /// </summary>
    public void ResetTimer()
    {
        if (scaled)
        {
            startTime = CoreTimeline.instance.ScaledTimeline;
        }
        else
        {
            startTime = Time.realtimeSinceStartup;
        }


        ResetTimer(targetTime);
    }

    /// <summary>
    /// Reset the timer and change the new target time.
    /// </summary>
    /// <param name="_newTargetTime">The new time to target</param>
    public void ResetTimer(float _newTargetTime)
    {
        ChangeTarget(_newTargetTime);

        if (scaled)
        {
            startTime = CoreTimeline.instance.ScaledTimeline;
        }
        else
        {
            startTime = Time.realtimeSinceStartup;
        }
    }

    /// <summary>
    /// Change the target time. Does not reset the active timer.
    /// </summary>
    /// <param name="_newTargetTime">The new time to target</param>
    public void ChangeTarget(float _newTargetTime)
    {
        targetTime = _newTargetTime;
    }

    /// <summary>
    /// Gets whether the timer has reached its target time
    /// </summary>
    /// <returns>Whether or not this timer has finished</returns>
    public bool TimerDone()
    {
        if(scaled)
        {
            return (CoreTimeline.instance.ScaledTimeline - startTime) >= targetTime;
        }
        else
        {
            return (Time.realtimeSinceStartup - startTime) >= targetTime;
        }
    }
}
