/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 29th, 2023
 * Last Edited - March 29th, 2023 by Ben Schuster
 * Description - Base class for a tooltip holder that will send a tooltip request to the manager
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TooltipHolder : MonoBehaviour
{
    public TooltipSO tooltip;
    protected TooltipManager manager;

    protected virtual void Start()
    {
        if (tooltip == null)
        {
            Destroy(this);
            return;
        }
        manager = TooltipManager.instance;
    }

    protected virtual void SubmitTooltip()
    {
        manager?.RequestTooltip(tooltip);
    }

    protected virtual void RetractTooltip()
    {
        manager?.UnloadTooltip(tooltip);
    }
}
