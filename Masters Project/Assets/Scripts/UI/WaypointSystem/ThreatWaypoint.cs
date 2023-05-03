using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ThreatWaypoint : GenericWaypointManager<Waypoint>
{
    public static ThreatWaypoint instance;

    protected override void Awake()
    {
        // Initialize self 
        if (instance == null)
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
