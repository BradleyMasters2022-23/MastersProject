using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrigger : Trigger
{
    [SerializeField] private float activationDelay;
    [SerializeField] private float timerDuration;
    private ScaledTimer timer;

    protected override void Awake()
    {
        base.Awake();

        StartCoroutine(DelayedStart());
    }

    private void LateUpdate()
    {
        if (timer != null && timer.TimerDone())
        {
            Activate();
            timer.ResetTimer();
        }
    }

    /// <summary>
    /// Delay the start before activating the timer
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayedStart()
    {
        ScaledTimer initDelay = new ScaledTimer(activationDelay);
        while(!initDelay.TimerDone())
            yield return null;

        // activate once before cooldown starts
        Activate();
        timer = new ScaledTimer(timerDuration);
    }
}
