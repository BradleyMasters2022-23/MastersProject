/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 5th, 2022
 * Last Edited - March 5th, 2022 by Ben Schuster
 * Description - Concrete target for regenerating shields
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTarget : Target
{
    /// <summary>
    /// Whether or not this shield is immediately active on the target
    /// </summary>
    private bool activeOnAwake = false;

    protected override void Awake()
    {
        activeOnAwake = gameObject.activeSelf;

        base.Awake();
    }

    public override void RegisterEffect(float dmg)
    {
        base.RegisterEffect(dmg);

        if(ShieldTooltipTrigger.instance != null)
            ShieldTooltipTrigger.instance.ShieldHit();

    }

    protected override void DestroyObject()
    {
        gameObject.SetActive(false);

        if (ShieldTooltipTrigger.instance != null)
            ShieldTooltipTrigger.instance.ShieldKilled();
    }

    public override void ResetTarget()
    {
        base.ResetTarget();

        // Set to immediately active state
        gameObject.SetActive(activeOnAwake);
    }
}
