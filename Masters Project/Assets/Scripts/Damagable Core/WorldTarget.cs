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
    [SerializeField] private Trigger onDamageTrigger;


    public override void RegisterEffect(float dmg)
    {
        if (onDamageTrigger != null)
            onDamageTrigger.Activate();

        base.RegisterEffect(dmg);
    }
}
