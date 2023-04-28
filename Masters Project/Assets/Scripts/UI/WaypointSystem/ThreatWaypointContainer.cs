using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatWaypointContainer : GenericWaypointContainer
{
    private ThreatWaypoint i;

    protected override Waypoint GetWaypoint(Transform targetOverride, WaypointData data)
    {
        i = ThreatWaypoint.instance;

        if (i == null)
        {
            return null;
        }
        

        return i.Request(targetOverride, data);
    }

    protected override void ReturnWaypoint()
    {
        if(wp != null)
            i.Return(wp);
    }
}
