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

    protected override void Awake()
    {
        base.Awake();
        onDamageTrigger= GetComponent<Trigger>();
    }

    public override void RegisterEffect(float dmg)
    {
        if (onDamageTrigger != null)
            onDamageTrigger.Activate();

        base.RegisterEffect(dmg);
    }
}
