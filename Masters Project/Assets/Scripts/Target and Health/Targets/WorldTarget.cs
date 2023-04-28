/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 3rd, 2022
 * Last Edited - February 3rd, 2022 by Ben Schuster
 * Description - Concrete target for world-based objects
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTarget : Target
{
    /// <summary>
    /// Trigger called when this target takes damage
    /// </summary>
    private Trigger onDamageTrigger;

    private bool triggerRegistered = false;

    protected override void Awake()
    {
        base.Awake();
        onDamageTrigger= GetComponent<Trigger>();
        triggerRegistered= true;
    }

    public override void RegisterEffect(float dmg)
    {
        if (dmg <= 0) return;

        if (onDamageTrigger != null)
            onDamageTrigger.Activate();

        base.RegisterEffect(dmg);
    }

    public bool IsTriggerRegistered()
    {
        return triggerRegistered;
    }
}
