using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimestopTooltip : TooltipHolder, TimeObserver
{
    [SerializeField] private float timeUntilTooltip;
    [SerializeField] private float minimumHoldTime;

    private ScaledTimer tooltipTracker;
    private ScaledTimer holdTimer;

    bool sent = false;

    int maxCount = 3;
    int useCaseCount = 0;

    protected override void Start()
    {
        base.Start();

        tooltipTracker = new ScaledTimer(timeUntilTooltip, false);
        holdTimer = new ScaledTimer(minimumHoldTime, false);
    }

    private void OnEnable()
    {
        TimeManager.instance.Subscribe(this);
    }
    private void OnDisable()
    {
        TimeManager.instance.UnSubscribe(this);
    }


    private void Update()
    {
        // turn off at max
        if(useCaseCount >= maxCount)
            this.enabled= false;

        if(!sent && !manager.HasTooltip(tooltip) && tooltipTracker.TimerDone())
        {
            SubmitTooltip();
            sent = true;
        }
        // if it was sent but not triggered normal dismiss, then reset timer
        else if(sent && !manager.HasTooltip(tooltip))
        {
            tooltipTracker.ResetTimer();
            sent = false;
        }
    }

    public void OnResume()
    {
        if (holdTimer.TimerDone())
        {
            useCaseCount++;
            RetractTooltip();
            tooltipTracker.ResetTimer();
        }
            
    }

    public void OnStop()
    {
        holdTimer.ResetTimer();
    }
}
