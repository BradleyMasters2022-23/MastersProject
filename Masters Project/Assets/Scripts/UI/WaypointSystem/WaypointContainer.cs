/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 17th, 2023
 * Last Edited - April 17th, 2023 by Ben Schuster
 * Description - Waypoint container that requests waypoints while active. 
 * Used to help create more custom waypoints easily
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointContainer : GenericWaypointContainer
{
    private WaypointManager i;

    protected override Waypoint GetWaypoint(Transform targetOverride, WaypointData data)
    {
        if (i == null)
        {
            i = WaypointManager.instance;
        }

        return i.Request(targetOverride, data);
    }

    protected override void ReturnWaypoint()
    {
        i.Return(wp);
    }
}
