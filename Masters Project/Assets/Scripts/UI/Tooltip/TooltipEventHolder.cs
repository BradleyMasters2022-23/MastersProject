using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipEventHolder : TooltipHolder
{
    public void CallTooltip()
    {
        Debug.Log("Trying to submit tooltip " + tooltip.name);
        SubmitTooltip();
    }
}
