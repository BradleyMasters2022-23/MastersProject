/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 17th, 2023
 * Last Edited - April 17th, 2023 by Ben Schuster
 * Description - Waypoint manager with a pooler and request functionality
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WaypointManager : GenericWaypointManager<Waypoint>
{
    public static WaypointManager instance;

    protected override void Awake()
    {
        // Initialize self 
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        base.Awake();
    }
}
