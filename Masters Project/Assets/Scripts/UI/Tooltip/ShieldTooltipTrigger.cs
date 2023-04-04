using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTooltipTrigger : TooltipHolder, ITriggerable
{
    [SerializeField] private Trigger shieldReference;
    private int triggerHitCount = 20;
    private int hitCount = 0;

    private void OnEnable()
    {
        if(shieldReference != null)
        {
            shieldReference.Register(this);
        }
            
    }

    private void IncrementTooltipRequirement()
    {
        hitCount++;

        if(hitCount == triggerHitCount)
        {
            SubmitTooltip();
        }
    }

    private void OnDisable()
    {
        if (shieldReference != null)
            shieldReference.Unregister(this);
        
        RetractTooltip();
    }

    public void Trigger()
    {
        IncrementTooltipRequirement();
    }
}
