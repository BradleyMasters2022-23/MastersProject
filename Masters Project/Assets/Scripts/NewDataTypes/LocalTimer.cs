/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - May 23th, 2022
 * Last Edited - May 23th, 2022 by Ben Schuster
 * Description - An independent timer class. Tracks its own time in a way that allows for localization of it.
 * ================================================================================================
 */
using UnityEngine;
using Masters.TimerSystem;

public class LocalTimer 
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
    /// The current time for this timer
    /// </summary>
    private float currentTime;

    /// <summary>
    /// Create a new local timer
    /// </summary>
    /// <param name="_targetTime"></param>
    /// <param name="_startTime"></param>
    public LocalTimer(float _targetTime)
    {
        currentTime = 0;
        startTime = currentTime;
        targetTime = _targetTime;
    }

    /// <summary>
    /// Reset the active timer.
    /// </summary>
    public void ResetTimer()
    {
        ResetTimer(targetTime);
    }

    /// <summary>
    /// Reset the timer and change the new target time.
    /// </summary>
    /// <param name="_newTargetTime">The new time to target</param>
    public void ResetTimer(float _newTargetTime)
    {
        startTime = currentTime;
        ChangeTarget(_newTargetTime);
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
        return (currentTime - startTime) >= (targetTime);
    }

    /// <summary>
    /// Get the progress of the timer on a 0 to 1 scale.
    /// 0 is not done. 1 is done.
    /// </summary>
    /// <returns>% Progress of time passed. </returns>
    public float TimerProgress()
    {
        return Mathf.Clamp((currentTime - startTime) / (targetTime), 0, 1);
    }

    /// <summary>
    /// Get the current target time for this timer
    /// </summary>
    /// <returns>The current target time</returns>
    public float CurrentTargetTime()
    {
        return targetTime;
    }

    public void UpdateTime(float _delta)
    {
        currentTime += _delta;
    }
}
